using Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            string[] r = new string[0];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                r = await Task.Run(() => Win_OpenFilePicker(ctx, options)); // Runs On Another Thread To Prevent Blockage
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
                if (options is FolderPickerOptions) {
                    var ptr = MacHelperLoader.Current.OpenFolder(options.AllowMultiSelection);
                    
                    //Read Pointer Until It's Different
                    while (StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
                    {
                        await Task.Delay(1);
                    }

                    r = ((string)StringFromNativeUtf8(ptr)).Split(':').Where(f => f.Length > 0).ToArray();

                    MacHelperLoader.Current.FreePointer(ptr); // We Don't Want Any Memory Leaks
                }
                else {

                    //Convert Array Into String By Appening :seperate: To Each Element
                    var extSplit = "";
                    for (int i = 0; i < options.AllowedFileTypes.Length; i++)
                    {
                        if (i != 0){
                            extSplit += ":seperate:";
                        }
                        extSplit += options.AllowedFileTypes[i];
                    }

                    var ptr = MacHelperLoader.Current.OpenFile(options.AllowMultiSelection, extSplit.ToLower());
                    
                    //Read Pointer Until It's Different
                    while (StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
                    {
                        await Task.Delay(1);
                    }

                    r = ((string)StringFromNativeUtf8(ptr)).Split(':').Where(f => f.Length > 0).ToArray(); ;

                    MacHelperLoader.Current.FreePointer(ptr); // We Don't Want Any Memory Leaks
                }
            }
            else {
                r = await GTKFilePicker.Open(ctx, options);
            }

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
                    //Yaay Easy To Read, Just Return The String
                    r = new string[] { val };
                }
                else
                {
                    //Annoying Oof

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
                var ptr = MacHelperLoader.Current.SaveFile(fileExtension);
                
                //Read Pointer Until It's Different
                while (StringFromNativeUtf8(ptr) == "nr") // Short For Not Returned
                {
                    await Task.Delay(1);
                }

                r = StringFromNativeUtf8(ptr);

                if (r == "null"){
                    r = null;
                }

                MacHelperLoader.Current.FreePointer(ptr); // We Don't Want Any Memory Leaks
            }
            else {
                r = await GTKFilePicker.Save(ctx, fileExtension);
            }
            

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

    public class GTKFilePicker
    {

        public static async Task<string> Save(DOM ctx, string extension)
        {
            var FnID = "gtkfunction-filesaver-" + Guid.NewGuid().ToString();
            JSEvent.PendingFunctions[FnID] = "JSI_NotReturned";


            Jobs.Push(AppManager.RunGTK, ctx);
            Application.Invoke(async (s, e) =>
            {

                FileChooserDialog filechooser = new FileChooserDialog("Save File", HelperWindow.Instance, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);

                var filter = new FileFilter();
                filter.Name = extension.ToUpper() + " Files";
                filter.AddPattern("*." + extension.ToLower());
                
                filechooser.AddFilter(filter);

                var result = filechooser.Run();
                if (result == (int)ResponseType.Accept)
                {
                    JSEvent.PendingFunctions[FnID] = filechooser.Files[0].ParsedName;
                }
                else
                {
                    JSEvent.PendingFunctions[FnID] = "";
                }

                filechooser.Destroy();
                HelperWindow.Instance.Destroy();
                
            });

            while (JSEvent.PendingFunctions[FnID] == "JSI_NotReturned")
            {
                await Task.Delay(1);
            }

            return JSEvent.PendingFunctions[FnID];
        }

        public static async Task<string[]> Open(DOM ctx, FilePickerOptions options)
        {
            var FnID = "gtkfunction-filepicker-" + Guid.NewGuid().ToString();
            JSEvent.PendingFunctions[FnID] = "JSI_NotReturned";


            Jobs.Push(AppManager.RunGTK, ctx);
            Application.Invoke(async (s, e) =>
            {
                if (options is FolderPickerOptions){
                    await FilePicker(ctx, options, FnID, FileChooserAction.SelectFolder);
                }
                else{
                    await FilePicker(ctx, options, FnID, FileChooserAction.Open);
                }
            });

            while (JSEvent.PendingFunctions[FnID] == "JSI_NotReturned")
            {
                await Task.Delay(1);
            }

            return JSEvent.PendingFunctions[FnID].Split(':').Where(f => f.Length > 0).ToArray();
        }


        static async Task FilePicker(DOM ctx, FilePickerOptions options, string FnID, FileChooserAction action){
            FileChooserDialog filechooser = new FileChooserDialog("Select File", HelperWindow.Instance, action, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

            if (options is FolderPickerOptions){

            }
            else{
                var filter = new FileFilter();
                filter.Name = "Files";
                foreach (var t in options.AllowedFileTypes)
                {
                    filter.AddPattern("*." + t);
                }
                
                filechooser.AddFilter(filter);
            }

            filechooser.SelectMultiple = options.AllowMultiSelection;

            var result = filechooser.Run();
            if (result == (int)ResponseType.Accept)
            {
                var fn = filechooser.Files;
                var tempres = "";
                
                foreach (var f in fn){
                    if (f == fn[0]){
                        tempres += f.ParsedName;
                    }
                    else {
                        tempres += ":" + f.ParsedName;
                    }
                    
                }
                JSEvent.PendingFunctions[FnID] = tempres;
            }
            else
            {
                JSEvent.PendingFunctions[FnID] = "";
            }

            filechooser.Destroy();
            HelperWindow.Instance.Destroy();
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
