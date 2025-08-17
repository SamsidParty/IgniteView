using IgniteView.Core;
using IgniteView.Core.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    /// <summary>
    /// Allows you to access the shared local storage of the application. 
    /// Warning 1: Modifying the local storage from C# will not automatically update the JavaScript side.  
    /// Warning 2: This is a synchronous wrapper around PersistentStorage, so it may block the UI thread if used excessively.  
    /// </summary>
    public class LocalStorage
    {
        private static PersistentStorage Storage => PlatformManager.Instance.Storage;

        private ConcurrentDictionary<string, string> Cache;
        private string FileName;
        private Task LoadTask;
        private WebWindow Host;

        private static string GetFileNameFromWindow(WebWindow ctx)
        {
            var url = "http://localhost/";
            if (ctx != null)
            {
                url = ctx.URL;
            }
            var host = new Uri(url).Host;

            return GetFileNameFromOrigin(host);
        }

        private static string GetFileNameFromOrigin(string origin)
        {
            if (string.IsNullOrEmpty(origin))
            {
                origin = "default";
            }
            if (origin.StartsWith("http:"))
            {
                origin = (new Uri(origin)).Host;
            }
            if (origin == "localhost" || origin == "127.0.0.1")
            {
                origin = "default";
            }

            return String.Join("_", origin.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.') + ".iv2_ls";
        }

        internal LocalStorage(WebWindow host)
        {
            Host = host;
            FileName = GetFileNameFromWindow(host);
            LoadTask = Load();
        }

        /// <summary>
        /// Creates a LocalStorage object from the specified origin. eg. https://samsidparty.com
        /// </summary>
        public LocalStorage(string origin)
        {
            FileName = GetFileNameFromOrigin(origin);
            LoadTask = Load();
        }

        #region Instance Methods

        private async Task Load()
        {
            var data = await Storage.ReadAllText(FileName);

            try {
                if (!string.IsNullOrEmpty(data))
                {
                    Cache = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(data);
                }
            }
            catch { }

            Cache ??= new ConcurrentDictionary<string, string>();
            Host?.CallFunction("window._localStorage.hydrate", Cache);
        }

        public async Task Save()
        {
            await LoadTask;
            await Storage.WriteAllText(FileName, JsonConvert.SerializeObject(Cache));
            Host?.CallFunction("window._localStorage.hydrate", Cache);
        }

        public async Task<string> GetItem(string itemName)
        {
            await LoadTask;
            return Cache[itemName];
        }

        public async Task<string[]> GetItemList()
        {
            await LoadTask;
            return Cache.Keys.ToArray();
        }

        public async Task<Dictionary<string, string>> GetAllItems()
        {
            await LoadTask;
            return Cache.ToDictionary();
        }

        public async Task SetItem(string itemName, string value)
        {

            if (value == null)
            {
                await RemoveItem(itemName);
                return;
            }

            await LoadTask;
            Cache[itemName] = value;
            await Save();
        }

        public async Task RemoveItem(string itemName)
        {
            await LoadTask;
            Cache.TryRemove(itemName, out _);
            await Save();
        }

        public async Task Clear()
        {
            await LoadTask;
            Cache.Clear();
            await Save();
        }

        #endregion

        #region Interop Methods

        [Command("igniteview_localstorage_get")]
        public static async Task<string> GetItem(string item, WebWindow ctx)
        {
            return await ctx.LocalStorage.GetItem(item);
        }

        [Command("igniteview_localstorage_get_all")]
        public static async Task<Dictionary<string, string>> GetAllItems(WebWindow ctx)
        {
            return await ctx.LocalStorage.GetAllItems();
        }

        [Command("igniteview_localstorage_list")]
        public static async Task<string[]> ListItems(WebWindow ctx)
        {
            return await ctx.LocalStorage.GetItemList();
        }

        [Command("igniteview_localstorage_set")]
        public static async Task SetItem(string item, string value, WebWindow ctx)
        {
            await ctx.LocalStorage.SetItem(item, value);
        }

        [Command("igniteview_localstorage_remove")]
        public static async Task RemoveItem(string item, WebWindow ctx)
        {
            await ctx.LocalStorage.RemoveItem(item);
        }

        [Command("igniteview_localstorage_clear")]
        public static async Task Clear(WebWindow ctx)
        {
            await ctx.LocalStorage.Clear();
        }

        #endregion
    }
}
