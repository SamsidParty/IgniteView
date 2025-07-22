using System.Runtime.InteropServices;

namespace IgniteView.FileDialogs;

internal static class NativeFunctions
{
    public const string LibraryName = "nfd";

    [DllImport(LibraryName)]
    public static extern NFDResult NFD_Init();
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_Quit();
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogU8(out string outPath, FileFilter[] filterList, int filterCount, string defaultPath);
     
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_OpenDialogMultipleU8(out IntPtr outPaths, FileFilter[] filterList, int filterCount, string defaultPath);
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PathSet_GetCount(IntPtr pathSet, out int count);
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PathSet_GetPathU8(IntPtr pathSet, int index, out string outPath);   
    
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_SaveDialogU8(out string outPath, FileFilter[] filterList, int filterCount, string defaultPath, string defaultName);
      
    [DllImport(LibraryName)]
    public static extern NFDResult NFD_PickFolderU8(out string outPath, string defaultPath);
    
    [DllImport(LibraryName)]
    public static extern string? NFD_GetError();
    
    [DllImport(LibraryName)]
    public static extern void NFD_ClearError();
}