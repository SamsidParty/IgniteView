using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IgniteView.FileDialogs;


public class FileFilter
{
    /// <summary>
    /// The name of the filter, which is displayed in the file dialog.
    /// </summary>
    public required string Name;

    /// <summary>
    /// The pattern of the filter, which specifies the file types to be displayed.
    /// Eg. "*.png;*.jpg" to show images
    /// </summary>
    public required string Pattern;

    [SetsRequiredMembers]
    public FileFilter(string name, string pattern)
    {
        Name = name;
        Pattern = pattern;
    }
}