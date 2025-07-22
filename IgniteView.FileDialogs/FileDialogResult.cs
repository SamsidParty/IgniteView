namespace IgniteView.FileDialogs;

public class FileDialogResult
{
    private readonly NFDResult result;

    public string Path { get; }

    public IReadOnlyList<string> Paths { get; }

    public bool IsError => result == NFDResult.NFD_ERROR;

    public string ErrorMessage { get; }

    public bool IsCancelled => result == NFDResult.NFD_CANCEL;

    public bool IsOk => result == NFDResult.NFD_OKAY;

    internal FileDialogResult(NFDResult result, string path, IReadOnlyList<string> paths, string errorMessage)
    {
        this.result = result;
        Path = path;
        Paths = paths;
        ErrorMessage = errorMessage;
    }
}