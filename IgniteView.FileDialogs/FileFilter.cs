using System.Runtime.InteropServices;

namespace IgniteView.FileDialogs;

[StructLayout(LayoutKind.Sequential)]
public struct FileFilter
{
    public string Name;
    public string Spec;
}