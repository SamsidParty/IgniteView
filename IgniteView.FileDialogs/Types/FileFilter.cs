using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IgniteView.FileDialogs;


public class FileFilter
{
    /// <summary>
    /// The name of the filter, which is displayed in the file dialog.
    /// </summary>
    public required string Name;

    /// <summary>
    /// The pattern of the filter, which specifies the file types to be displayed.
    /// You may include multiple file types separated by commas.
    /// Eg. "png,jpg,webp" to show images
    /// </summary>
    public required string Pattern;

    // Convert filter to array with 1 element
    public static implicit operator FileFilter[](FileFilter f) => new FileFilter[] { f };

    /// <summary>
    /// Creates a new file filter with the specified name and pattern.
    /// </summary>
    /// <param name="name">The name of the filter, which is displayed in the file dialog.</param>
    /// <param name="pattern">The list of file extensions, seperated by a comma. Eg. "png,jpg,webp"</param>
    [SetsRequiredMembers]
    public FileFilter(string name, string pattern)
    {
        Name = name;
        Pattern = pattern;
    }

    /// <summary>
    /// Creates a new file filter with a single file extension
    /// </summary>
    [SetsRequiredMembers]
    public FileFilter(string fileExtension)
    {
        if (fileExtension.StartsWith("."))
        {
            fileExtension = fileExtension[1..]; // Remove leading dot
        }

        Name = fileExtension.ToUpper() + " Files";
        Pattern = fileExtension;
    }
}