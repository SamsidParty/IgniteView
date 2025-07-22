using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs;

public static class FileDialog
{


    public static string PickFile(string defaultPath, Dictionary<string, string> filterList) => OpenDialogU8(defaultPath, filterList);

    public static string PickFile(string defaultPath) => PickFile(defaultPath, new Dictionary<string, string>());
    public static string PickFile() => PickFile("");

    public static string[] PickMultipleFiles(string defaultPath, Dictionary<string, string> filterList) => OpenDialogMultipleU8(defaultPath, filterList);

    public static string[] PickMultipleFiles(string defaultPath) => PickMultipleFiles(defaultPath, new Dictionary<string, string>());
    public static string[] PickMultipleFiles() => PickMultipleFiles("");

    public static string SaveFile(string defaultPath, string defaultName, Dictionary<string, string> filterList) => SaveDialogU8(defaultPath, defaultName, filterList);

    public static string SaveFile(string defaultPath, string defaultName) => SaveFile(defaultPath, defaultName, new Dictionary<string, string>());
    public static string SaveFile(string defaultName) => SaveFile("", defaultName);

    public static string PickFolder(string defaultPath) => PickFolderU8(defaultPath);
    public static string PickFolder() => PickFolderU8("");

    #region Internal Methods

    private static string PickFolderU8(string defaultPath)
    {
        ImportResolver.Initialize();
        NativeFunctions.NFD_PickFolderU8(out var path,
            defaultPath).ThrowOnError();
        NativeFunctions.NFD_Quit(); return path;
    }


    private static string SaveDialogU8(string defaultPath, string defaultName, Dictionary<string, string> filterList)
    {
        ImportResolver.Initialize();
        NativeFunctions.NFD_SaveDialogU8(out var path, filterList.ToFilterListU8(),
            filterList.Count, defaultPath, defaultName).ThrowOnError();
        NativeFunctions.NFD_Quit(); return path;
    }


    private static string[] OpenDialogMultipleU8(string defaultPath, Dictionary<string, string> filterList)
    {
        ImportResolver.Initialize();
        NativeFunctions.NFD_OpenDialogMultipleU8(out var ptr, filterList.ToFilterListU8(),
            filterList.Count, defaultPath).ThrowOnError();
        NativeFunctions.NFD_PathSet_GetCount(ptr, out var count);
        var array = new string[count];
        for (var i = 0; i < count; i++)
        {
            NativeFunctions.NFD_PathSet_GetPathU8(ptr, i, out var path);
            array[i] = path;
        }

        NativeFunctions.NFD_Quit();
        return array;
    }


    private static string OpenDialogU8(string defaultPath, Dictionary<string, string> filterList)
    {
        ImportResolver.Initialize();
        NativeFunctions.NFD_OpenDialogU8(out var path, filterList.ToFilterListU8(),
            filterList.Count, defaultPath).ThrowOnError();
        NativeFunctions.NFD_Quit(); return path;
    }

    // Throws an exception on error
    private static void ThrowOnError(this NFDResult result)
    {
        if (result == NFDResult.NFD_ERROR)
            throw new Exception(GetError());
    }

    private static string? GetError()
    {
        var error = NativeFunctions.NFD_GetError();
        NativeFunctions.NFD_ClearError(); return error;
    }

    // Converts dictionary to filter list
    private static FileFilter[] ToFilterListU8(this Dictionary<string, string> dict)
    {
        var list = dict.ToList();
        var filters = new FileFilter[dict.Count];
        for (var i = 0; i < filters.Length; i++)
            filters[i] = new FileFilter
            {
                Name = list[i].Key,
                Spec = list[i].Value
            };
        return filters;
    }
    
    #endregion
}