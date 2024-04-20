using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    public class WinHelper
    {

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetOpenFileName(ref OpenFileName ofn);

        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetSaveFileName(ref OpenFileName ofn);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OpenFileName
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int flagsEx;
        }

        public void OnLoad()
        {
            Logger.LogInfo("Loaded WinHelper");
        }

        public async Task<string> OpenFileSaver(DOM ctx, string fileExtension)
        {
            var r = "";

            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);

            ofn.lpstrFilter = fileExtension.ToUpper() + " Files\0*." + fileExtension + "\0\0";

            ofn.lpstrFile = Marshal.StringToBSTR(new string(' ', 1024)); // 1KB
            ofn.nMaxFile = 1024;
            ofn.lpstrFileTitle = new string(new char[1024]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Save File";


            if (GetSaveFileName(ref ofn))
            {
                IntPtr ptr = ofn.lpstrFile;
                string val = Marshal.PtrToStringUni(ptr);

                r = val;
            }

            Marshal.FreeBSTR(ofn.lpstrFile);

            return r;
        }

        //WARNING: Extremely Long Function
        public async Task<string[]> OpenFilePicker(DOM ctx, FilePickerOptions options)
        {
            var r = new string[0];

            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);

            ofn.lpstrFilter += "All Accepted Files\0";
            for (var i = 0; i < options.AllowedFileTypes.Length; i++)
            {
                var ext = options.AllowedFileTypes[i];
                if (i != 0)
                {
                    ofn.lpstrFilter += ";";
                }
                ofn.lpstrFilter += $"*.{ext}";
            }
            ofn.lpstrFilter += "\0";

            for (var i = 0; i < options.AllowedFileTypes.Length; i++)
            {
                var ext = options.AllowedFileTypes[i];
                ofn.lpstrFilter += $"{ext.ToUpper()} Files\0*.{ext}\0";
            }
            ofn.lpstrFilter += "\0";

            ofn.lpstrFile = Marshal.StringToBSTR(new string(' ', 1024 * 1024)); // 1MB
            ofn.nMaxFile = 1024 * 1024;
            ofn.lpstrFileTitle = new string(new char[1024]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Select File";

            if (options.AllowMultiSelection)
            {
                ofn.Flags += 0x00000200; // OFN_ALLOWMULTISELECT
                ofn.Flags += 0x00080000; // OFN_EXPLORER
                ofn.Flags += 0x00001000; // OFN_FILEMUSTEXIST
            }

            if (GetOpenFileName(ref ofn))
            {
                IntPtr ptr = ofn.lpstrFile;
                string val = Marshal.PtrToStringUni(ptr);

                //Check If It's In The Easy Format Or The Annoying Format
                if (File.Exists(val))
                {
                    //Easy To Read, Just Return The String
                    r = new string[] { val };
                }
                else
                {
                    //Annoying

                    var files = new List<string>();

                    //Read Until Null, Repeat Until Double Null
                    while (true)
                    {
                        string str = Marshal.PtrToStringUni(ptr);
                        if (str.Length == 0)
                            break;

                        if (str != val)
                        {
                            files.Add(Path.Combine(val, str)); // Directory + File Name
                        }

                        ptr = new IntPtr(ptr.ToInt64() + (str.Length + 1 /* NULL */) * sizeof(char));
                    }

                    r = files.ToArray();
                }

            }

            Marshal.FreeBSTR(ofn.lpstrFile);

            return r;
        }
    }
}
