using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.Core
{
    public class LocalStorage
    {
        public static string GetBasePath()
        {
            var basePath = Path.Combine(AppManager.Instance.CurrentIdentity.AppDataPath, "LocalStorage");
            Directory.CreateDirectory(basePath);
            return basePath;
        }

        public static string GetPathOfItem(string itemName)
        {
            var sanitized = String.Join("_", itemName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return Path.Join(GetBasePath(), sanitized + ".iv2_ls");
        }

        [Command("igniteview_localstorage_get")]
        public static string GetItem(string itemName)
        {
            var path = GetPathOfItem(itemName);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        [Command("igniteview_localstorage_list")]
        public static string[] GetItemList()
        {
            var basePath = GetBasePath();
            return Directory.GetFiles(basePath, "*.iv2_ls").Select((f) => Path.GetFileNameWithoutExtension(f)).ToArray();
        }

        [Command("igniteview_localstorage_get_all")]
        public static Dictionary<string, string> GetAllItems()
        {
            var items = new Dictionary<string, string>();
            foreach (var key in GetItemList())
            {
                items[key] = GetItem(key);
            }
            return items;
        }

        [Command("igniteview_localstorage_set")]
        public static void SetItem(string itemName, string value)
        {
            var path = GetPathOfItem(itemName);
            File.WriteAllText(path, value);
        }

        [Command("igniteview_localstorage_remove")]
        public static void RemoveItem(string itemName)
        {
            var path = GetPathOfItem(itemName);
            File.Delete(path);
        }

        [Command("igniteview_localstorage_clear")]
        public static void Clear()
        {
            foreach (var item in GetAllItems())
            {
                RemoveItem(item.Key);
            }
        }
    }
}
