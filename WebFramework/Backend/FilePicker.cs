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


        public static bool IsOpen = false;


        ///<summary>
        ///Prompts The User To Select File(s), Returns An Array Of Selected File(s), Returns An Empty Array If Cancelled By User
        ///</summary>
        public static async Task<string[]> OpenFilePicker(DOM ctx, FilePickerOptions options)
        {
            if (IsOpen) { return new string[0]; } IsOpen = true;

            Logger.LogInfo("Opening File Picker");

            string[] r = new string[0];

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Platform.isWindowsPT)
            {
                r = await Task.Run(async () => await WinHelperLoader.Current.OpenFilePicker(ctx, options)); // Runs On Another Thread To Prevent Blockage
            }
            else if (Platform.isUWP)
            {
                r = await UWPHelperLoader.Current.OpenFilePicker(options);
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



        ///<summary>
        ///Prompts The User To Select A Folder, Returns An Empty String If Cancelled By User
        ///</summary>
        public static async Task<string> OpenFolderPicker(DOM ctx)
        {
            var picked = await OpenFilePicker(ctx, new FolderPickerOptions() { AllowMultiSelection = false });
            if (picked.Length > 0)
            {
                return picked[0];
            }
            return "";
        }

        ///<summary>
        ///Prompts The User To Save A File, The Returned Path Might Not Exist, Returns An Empty String If Cancelled By User, File Extension Should Not Contain A Period
        ///</summary>
        public static async Task<string> OpenFileSaver(DOM ctx, string fileExtension)
        {
            if (IsOpen) { return null; } IsOpen = true;

            string r;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Platform.isWindowsPT)
            {
                r = await Task.Run(async () => await WinHelperLoader.Current.OpenFileSaver(ctx, fileExtension));
            }
            else if (Platform.isUWP)
            {
                r = await UWPHelperLoader.Current.OpenFileSaver(fileExtension);
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
