# Restricting The Size

Every `WebWindow` has a `Bounds` property of type `WindowBounds`, which allows you to fine tune the sizing of the window.  


You can construct a basic `WindowBounds` object with a `width` and `height` parameter:
```csharp title="Program.cs"
// Creates a WindowBounds object with width 640 and height 480
// When the window is displayed, it will be 640x480 pixels
// This will have no minimum or maximum size
var bounds = new WindowBounds(640, 480);
```

Applying these bounds to the window is simple:

```csharp title="Program.cs"
var mainWindow = WebWindow.Create().Show();
mainWindow.Bounds = bounds;
```

And with method chaining:

```csharp title="Program.cs"
var mainWindow =
    WebWindow.Create()
    .WithBounds(bounds)
    .Show();
```

# Minimum & Maximum Size

The `WindowBounds` object has properties to control the minimum and maximum size of the window:
- MinWidth
- MinHeight
- MaxWidth
- MaxHeight

Using them is trivial:

```csharp title="Program.cs"
bounds.MinWidth = 400;
bounds.MinHeight = 400;
bounds.MaxWidth = 1920;
bounds.MaxHeight = 1080;
```

Or directly from the constructor:

```csharp title="Program.cs"
var bounds = new WindowBounds(1280, 720) // 1280x720 represents the initial size
{
    MinWidth = 400,
    MinHeight = 400,
    MaxWidth = 1920,
    MaxHeight = 1080,
};
```