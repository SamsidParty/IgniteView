using IgniteView.FileDialogs;
using System.Runtime.InteropServices;

namespace IgniteView.FileDialogs.Desktop;

internal static class NFDMethods
{
    public const string LibraryName = "nfd";

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_Init();
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_Quit();
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogU8(out string outPath, NFDFilterU8[] filterList, int filterCount, string defaultPath);

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogN(
        [MarshalAs(UnmanagedType.LPWStr)] out string outPath,
        NFDFilterN[] filterList,
        int filterCount,
        [MarshalAs(UnmanagedType.LPWStr)] string defaultPath
    );

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogMultipleU8(out nint outPaths, NFDFilterU8[] filterList, int filterCount, string defaultPath);

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogMultipleN(
        out IntPtr outPaths,
        NFDFilterN[] filterList,
        int filterCount,
        [MarshalAs(UnmanagedType.LPWStr)] string defaultPath
    );

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PathSet_GetCount(nint pathSet, out int count);
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PathSet_GetPathU8(nint pathSet, int index, out string outPath);

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PathSet_GetPathN(
        IntPtr pathSet,
        int index,
        [MarshalAs(UnmanagedType.LPWStr)] out string outPath
    );

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_SaveDialogU8(out string outPath, NFDFilterU8[] filterList, int filterCount, string defaultPath, string defaultName);

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_SaveDialogN(
    [MarshalAs(UnmanagedType.LPWStr)] out string outPath,
        NFDFilterN[] filterList,
        int filterCount,
        [MarshalAs(UnmanagedType.LPWStr)] string defaultPath,
        [MarshalAs(UnmanagedType.LPWStr)] string defaultName
    );

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PickFolderU8(out string outPath, string defaultPath);

    [DllImport("nfd")]
    public static extern NFDResult NFD_PickFolderN(
        [MarshalAs(UnmanagedType.LPWStr)] out string outPath,
        [MarshalAs(UnmanagedType.LPWStr)] string defaultPath
    );

    [DllImport(LibraryName)]
    public static extern string? NFD_GetError();
    
    [DllImport(LibraryName)]
    public static extern void NFD_ClearError();

    internal static string GetImportName(string baseImportName)
    {
        return baseImportName;
    }
}