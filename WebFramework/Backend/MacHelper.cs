using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
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

        [DllImport("IgniteViewMac", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Test();

        public static void Init(){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            InitWindow(WindowManager.Options.TitlebarColor.Value.R, WindowManager.Options.TitlebarColor.Value.G, WindowManager.Options.TitlebarColor.Value.B);
        }

        public static void OnTitleChanged(string t){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            UpdateTitle(t);
        }

        public static void OnIconChanged(string path){
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            if (!File.Exists(path)) { return; }
            UpdateIcon(path);
        }
    }
}
