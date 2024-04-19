using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    public class MacHelper
    {
        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreePointer(IntPtr address);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitWindow(int r, int g, int b);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UpdateTitle(string title);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UpdateIcon(string title);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SaveFile(string extension);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenFolder(bool multiSelect);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenFile(bool multiSelect, string allowedTypes);

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsDark();

        private static MacHelper _Current;

        public static MacHelper Current
        {
            get
            {
                if (_Current == null) { _Current = new MacHelper(); }
                return _Current;
            }
        }

        public async Task<string> OpenFileSaver(string fileExtension)
        {
            string r;
            var ptr = SaveFile(fileExtension);

            //Read Pointer Until It's Different
            while (FilePicker.StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
            {
                await Task.Delay(1);
            }

            r = FilePicker.StringFromNativeUtf8(ptr);

            if (r == "null")
            {
                r = null;
            }

            FreePointer(ptr); // We Don't Want Any Memory Leaks

            return r;
        }

        public async Task<string[]> OpenFilePicker(FilePickerOptions options)
        {
            string[] r;
            if (options is FolderPickerOptions)
            {
                var ptr = OpenFolder(options.AllowMultiSelection);

                //Read Pointer Until It's Different
                while (FilePicker.StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
                {
                    await Task.Delay(1);
                }

                r = (FilePicker.StringFromNativeUtf8(ptr)).Split(':').Where(f => f.Length > 0).ToArray();

                FreePointer(ptr); // We Don't Want Any Memory Leaks
            }
            else
            {

                //Convert Array Into String By Appending :seperate: To Each Element
                var extSplit = "";
                for (int i = 0; i < options.AllowedFileTypes.Length; i++)
                {
                    if (i != 0)
                    {
                        extSplit += ":seperate:";
                    }
                    extSplit += options.AllowedFileTypes[i];
                }

                var ptr = OpenFile(options.AllowMultiSelection, extSplit.ToLower());

                //Read Pointer Until It's Different
                while (FilePicker.StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
                {
                    await Task.Delay(1);
                }

                r = (FilePicker.StringFromNativeUtf8(ptr)).Split(':').Where(f => f.Length > 0).ToArray(); ;

               FreePointer(ptr); // We Don't Want Any Memory Leaks
            }
            return r;
        }

        public bool IsDarkMode()
        {
            return IsDark();
        }

        public void Init(){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            InitWindow(WindowManager.Options.TitlebarColor.Value.R, WindowManager.Options.TitlebarColor.Value.G, WindowManager.Options.TitlebarColor.Value.B);
        }

        public void OnTitleChanged(string t){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            UpdateTitle(t);
        }

        public void OnIconChanged(string path){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            if (!File.Exists(path)) { return; }
            UpdateIcon(path);
        }


        //Called By WebFramework
        public void OnLoad()
        {
            Logger.LogInfo("MacHelper Is Loaded And Active");
        }
    }
}
