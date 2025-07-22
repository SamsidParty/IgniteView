using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgniteView.FileDialogs.Desktop;

internal static class NFDBindings
{
    internal static string PickFolderU8(string defaultPath)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_PickFolderU8(out var path,
            defaultPath).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    internal static string PickFolderN(string defaultPath)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_PickFolderN(out var path,
            defaultPath).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    internal static string SaveDialogU8(string defaultPath, string defaultName, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_SaveDialogU8(out var path, filterList.ToFilterListU8(),
            filterList.Count, defaultPath, defaultName).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    internal static string SaveDialogN(string defaultPath, string defaultName, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_SaveDialogN(out var path, filterList.ToFilterListN(),
            filterList.Count, defaultPath, defaultName).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    internal static string[] OpenDialogMultipleU8(string defaultPath, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_OpenDialogMultipleU8(out var ptr, filterList.ToFilterListU8(),
            filterList.Count, defaultPath).ThrowOnError();
        NFDMethods.NFD_PathSet_GetCount(ptr, out var count);
        var array = new string[count];
        for (var i = 0; i < count; i++)
        {
            NFDMethods.NFD_PathSet_GetPathU8(ptr, i, out var path);
            array[i] = path;
        }

        NFDMethods.NFD_Quit();
        return array;
    }

    internal static string[] OpenDialogMultipleN(string defaultPath, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_OpenDialogMultipleN(out var ptr, filterList.ToFilterListN(),
            filterList.Count, defaultPath).ThrowOnError();
        NFDMethods.NFD_PathSet_GetCount(ptr, out var count);
        var array = new string[count];
        for (var i = 0; i < count; i++)
        {
            NFDMethods.NFD_PathSet_GetPathN(ptr, i, out var path);
            array[i] = path;
        }

        NFDMethods.NFD_Quit();
        return array;
    }

    internal static string OpenDialogU8(string defaultPath, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_OpenDialogU8(out var path, filterList.ToFilterListU8(),
            filterList.Count, defaultPath).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    internal static string OpenDialogN(string defaultPath, Dictionary<string, string> filterList)
    {
        NFDImportResolver.Initialize();
        NFDMethods.NFD_OpenDialogN(out var path, filterList.ToFilterListN(),
            filterList.Count, defaultPath).ThrowOnError();
        NFDMethods.NFD_Quit(); return path;
    }

    // Throws an exception on error
    internal static void ThrowOnError(this NFDResult result)
    {
        if (result == NFDResult.NFD_ERROR)
            throw new Exception(GetError());
    }

    internal static string? GetError()
    {
        var error = NFDMethods.NFD_GetError();
        NFDMethods.NFD_ClearError(); return error;
    }

    // Converts dictionary to filter list
    internal static NFDFilterU8[] ToFilterListU8(this Dictionary<string, string> dict)
    {
        var list = dict.ToList();
        var filters = new NFDFilterU8[dict.Count];
        for (var i = 0; i < filters.Length; i++)
            filters[i] = new NFDFilterU8 { Name = list[i].Key, Spec = list[i].Value };
        return filters;
    }

    internal static NFDFilterN[] ToFilterListN(this Dictionary<string, string> dict)
    {
        var list = dict.ToList();
        var filters = new NFDFilterN[dict.Count];
        for (var i = 0; i < filters.Length; i++)
            filters[i] = new NFDFilterN { Name = list[i].Key, Spec = list[i].Value };
        return filters;
    }

}