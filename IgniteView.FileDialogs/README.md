# IgniteView File Dialogs

This package allows you to open native file dialogs for the following actions:

- Pick file
- Pick multiple files
- Save file
- Pick folder

It is designed to integrate well with the IgniteView framework, however it can still be used separately.

## Supported Platforms
- 🪟 Windows x64
- 🪟 Windows arm64
- 🐧 Linux x64
- 🐧 Linux arm64
- 🍎 macOS arm64
- 🍎 macOS x64 (May be removed in the future)

⚠️ 32-bit operating systems are not supported

## Usage Examples

Pick any file:
```csharp
string filePath = FileDialog.PickFile();
```

Pick PNG file:
```csharp
string filePath = FileDialog.PickFile(new FileFilter("png"));
```

Pick image:
```csharp
string filePath = FileDialog.PickFile(new FileFilter("Images", "png,jpg,webp,gif"));
```

Pick multiple PNG files:
```csharp
string[] filePaths = FileDialog.PickMultipleFiles(new FileFilter("png"));
```

Save MP4 file:
```csharp
string filePath = FileDialog.SaveFile(new FileFilter("mp4"));
```

Pick folder:
```csharp
string folderPath = FileDialog.PickFolder();
```