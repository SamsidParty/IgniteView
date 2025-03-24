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

## Minimum & Maximum Size

The `WindowBounds` object has properties to control the minimum and maximum size of the window:
- `MinWidth`
- `MinHeight`
- `MaxWidth`
- `MaxHeight`

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

You can also disable the maximum size entirely, which allows the window to be as big as the user's monitor.
This is the recommended approach as some users with high resolution monitors will require large windows.

```csharp title="Program.cs"
// Set the maximum size to 0, which will disable it entirely
bounds.MaxWidth = 0;
bounds.MaxHeight = 0;
```

## Locked Window Bounds

You can set the `Bounds` of a `WebWindow` to a `LockedWindowBounds` object as well, which will disable resizing, maximizing, and minimizing.

```csharp title="Program.cs"
// The window will be locked to 1280x720
var bounds = new LockedWindowBounds(1280, 720);
mainWindow.Bounds = bounds;
```