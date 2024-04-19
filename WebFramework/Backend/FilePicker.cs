using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebFramework.Backend;

namespace WebFramework
{
    public class FilePicker
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

        public static bool IsOpen = false;


        ///<summary>
        ///Prompts The User To Select File(s), Returns An Array Of Selected File(s), Returns An Empty Array If Cancelled By User
        ///</summary>
        public static async Task<string[]> OpenFilePicker(DOM ctx, FilePickerOptions options)
        {
            if (IsOpen) { return new string[0]; } IsOpen = true;

            Logger.LogInfo("Opening File Picker");

            string[] r = new string[0];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                r = await Task.Run(() => Win_OpenFilePicker(ctx, options)); // Runs On Another Thread To Prevent Blockage
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
                r = await MacHelperLoader.Current.OpenFilePicker(options);
            }
            else {
                //TODO: Open File Picker
                r = null;
            }


            Logger.LogInfo("File Picker Returned " + JsonConvert.SerializeObject(r));

            IsOpen = false;
            return r;
        }

        //WARNING: Extremely Long Function
        static async Task<string[]> Win_OpenFilePicker(DOM ctx, FilePickerOptions options)
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
                ofn.lpstrFilter +=  $"{ext.ToUpper()} Files\0*.{ext}\0";
            }
            ofn.lpstrFilter += "\0";

            ofn.lpstrFile = Marshal.StringToBSTR(new string(' ', 1024*1024)); // 1MB
            ofn.nMaxFile = 1024*1024;
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

        ///<summary>
        ///Prompts The User To Select Folder(s), Returns An Array Of Selected Folder(s), Returns An Empty Array If Cancelled By User
        ///</summary>
        public static async Task<string[]> OpenFolderPicker(DOM ctx, FolderPickerOptions options)
        {
            return await OpenFilePicker(ctx, options);
        }

        ///<summary>
        ///Prompts The User To Save A File, The Returned Path Might Not Exist, Returns null If Cancelled By User, File Extension Should Not Contain A Period
        ///</summary>
        public static async Task<string> OpenFileSaver(DOM ctx, string fileExtension)
        {
            if (IsOpen) { return null; } IsOpen = true;

            string r;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                r = await Task.Run(() => Win_OpenFileSaver(ctx, fileExtension));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                r = await MacHelperLoader.Current.OpenFileSaver(fileExtension);
            }
            else {
                //TODO: File Picker
                r = null;
            }

            Logger.LogInfo("File Saver Returned " + r);

            IsOpen = false;
            return r;
        }

        // https://stackoverflow.com/a/10773988/18071273
        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            int len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        static async Task<string> Win_OpenFileSaver(DOM ctx, string fileExtension)
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
    }


    public class FileType : List<string> {

        //Members
        List<string> Data;

        public FileType(List<string> data){
            Data = data;
        }

        public static FileType VectorImage = new string[] {
            "svg",
            "dxf",
            "eps",
            "ai"
        };

        public static FileType RasterImage = new string[] {
            "apng",
            "avif",
            "gif",
            "jpg",
            "jpeg",
            "jfif",
            "pjpeg",
            "pjp",
            "png",
            "webp",
            "bmp"
        };

        public static FileType Video = new string[] {
            "webm",
            "mkv",
            "mpg",
            "mpeg",
            "h264",
            "ogg",
            "oggv",
            "avi",
            "mov",
            "qt",
            "wmv",
            "mp4",
            "m4a"
        };

        public static FileType MSOffice = new string[] {
            "doc",
            "ppt",
            "xls",
            "docx",
            "pptx",
            "xlsx"
        };

        public static FileType Document = MSOffice + "pdf" + "rtf";

        public static FileType Image = VectorImage + RasterImage;

        //Single Types
        public static FileType AllFiles = "*";
        public static FileType Text = "txt";
        public static FileType JSON = "json";
        public static FileType PDF = "pdf";
        public static FileType XML = "xml" + "xaml";
        public static FileType HTML = "htm" + "html";




        //Operators & Converters

        public static implicit operator FileType(string[] ar) => new FileType(ar.ToList<string>());
        public static implicit operator FileType(string v) => new FileType(new List<string>() { v });
        public static implicit operator string[](FileType t) => t.Data.ToArray();

        public static FileType operator +(FileType a, FileType b) {
            var ls = new List<string>();
            ls.AddRange(a.Data);
            ls.AddRange(b.Data);
            return new FileType(ls.Distinct().ToList());
        }

        public static FileType operator +(FileType a, string b) {
            var ls = new List<string>();
            ls.AddRange(a.Data);
            ls.Add(b);
            return new FileType(ls.Distinct().ToList());
        }
    }

    public class FilePickerOptions
    {
        /// <summary>
        /// An Array Of Allowed File Types, Without A Leading Period
        /// </summary>
        public string[] AllowedFileTypes = new string[] { "*" };

        public bool AllowMultiSelection;
    }

    public class FolderPickerOptions : FilePickerOptions {}

}
