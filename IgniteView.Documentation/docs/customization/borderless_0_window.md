# Removing The Titlebar

With IgniteView, it's trivial to create a frameless window:

```csharp title="Program.cs"
var mainWindow =
    WebWindow.Create()
    .WithTitle("You can't see this title!")
    .WithoutTitleBar()
    .Show();
```

And to re-enable the titlebar, it's just as easy:

```csharp title="Program.cs"
mainWindow.WithTitleBar();
```

The `WithTitleBar` method has an overload that takes a `bool`, so you can also dynamically choose whether to enable the titlebar or not.

```csharp title="Program.cs"
var mainWindow =
    WebWindow.Create()
    .WithTitle("You can only see this title on windows!")
    .WithTitleBar(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Only show the titlebar on windows
    .Show();
```