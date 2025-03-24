# Changing The Title

You can change the title of the window after creation using the `Title` property of the `WebWindow` object.

```csharp title="Program.cs"
var mainWindow = WebWindow.Create().Show();
mainWindow.Title = "New Title!";
```

With method chaining, you can also use the `WithTitle` method of the `WebWindow` to set the title when creating the window.
This is the recommended method of setting the title.

```csharp title="Program.cs"
var mainWindow =
    WebWindow.Create()
    .WithTitle("New Title!")
    .Show();
```