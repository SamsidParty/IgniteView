using IgniteView.Core;
using IgniteView.Core.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        internal static PersistentStorage Storage => PlatformManager.Instance.Storage;
        internal static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> Cache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private static string GetPathFromWindow(WebWindow ctx)
        {
            var url = "http://localhost/";
            if (ctx != null)
            {
                url = ctx.URL;
            }
            var host = new Uri(url).Host;

            return GetPathFromHost(host);
        }

        private static string GetPathFromHost(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                host = "default";
            }
            if (host.StartsWith("http:"))
            {
                host = (new Uri(host)).Host;
            }
            if (host == "localhost" || host == "127.0.0.1")
            {
                host = "default";
            }

            var path = String.Join("_", host.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.') + ".iv2_ls";

            if (!Cache.ContainsKey(path))
            {
                Cache[path] = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(Storage.ReadAllText(path).Result ?? "{}")!;
            }

            return path;
        }

        public static void Save()
        {
            foreach (var ls in Cache)
            {
                Storage.WriteAllText(ls.Key, JsonConvert.SerializeObject(ls.Value)); // Don't await the task
            }
        }

        [Command("igniteview_localstorage_get")]
        public static string GetItem(string itemName, WebWindow ctx)
        {
            return Cache[GetPathFromWindow(ctx)][itemName];
        }

        public static string GetItem(string itemName, string host)
        {
            return Cache[GetPathFromHost(host)][itemName];
        }

        [Command("igniteview_localstorage_list")]
        public static string[] GetItemList(WebWindow ctx)
        {
            return Cache[GetPathFromWindow(ctx)].Keys.ToArray();
        }

        public static string[] GetItemList(string host)
        {
            return Cache[GetPathFromHost(host)].Keys.ToArray();
        }

        [Command("igniteview_localstorage_get_all")]
        public static Dictionary<string, string> GetAllItems(WebWindow ctx)
        {
            return Cache[GetPathFromWindow(ctx)].ToDictionary();
        }

        public static Dictionary<string, string> GetAllItems(string host)
        {
            return Cache[GetPathFromHost(host)].ToDictionary();
        }

        [Command("igniteview_localstorage_set")]
        public static void SetItem(string itemName, string value, WebWindow ctx)
        {
            Cache[GetPathFromWindow(ctx)][itemName] = value;
            Save();
        }

        public static void SetItem(string itemName, string value, string host)
        {
            Cache[GetPathFromHost(host)][itemName] = value;
            Save();
        }

        [Command("igniteview_localstorage_remove")]
        public static void RemoveItem(string itemName, WebWindow ctx)
        {
            Cache[GetPathFromWindow(ctx)].TryRemove(itemName, out _);
            Save();
        }

        public static void RemoveItem(string itemName, string host)
        {
            Cache[GetPathFromHost(host)].TryRemove(itemName, out _);
            Save();
        }

        [Command("igniteview_localstorage_clear")]
        public static void Clear(WebWindow ctx)
        {
            Cache[GetPathFromWindow(ctx)] = new ConcurrentDictionary<string, string>();
            Save();
        }

        public static void Clear(string host)
        {
            Cache[GetPathFromHost(host)] = new ConcurrentDictionary<string, string>();
            Save();
        }
    }
}
