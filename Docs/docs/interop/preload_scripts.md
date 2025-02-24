# Preload Scripts

With IgniteView, you can define multiple preload scripts, which are injected into the WebWindow on page load.

## Creating A Preload Script

A preload script is simply a `.js` file in the web content directory. You can start by creating a `preload.js` file in the web root:

```javascript title="preload.js"
alert("Preload script works!");
```

For vite projects:
    - Add the `preload.js` file to `src-vite/public/preload.js`
    - Make sure the JavaScript file is in the `public` directory, otherwise vite will transform the file

For raw HTML projects:
    - Add the `preload.js` file to `wwwroot/preload.js`

## Registering The Preload Script

You can use the `RegisterPreloadScriptFromPath` method on the `AppManager` to register your preload script.
This method must be called *before* any windows are created, and *after* the `AppManager` has been created.

```csharp title="Program.cs"
// Construct AppManager (app) here

app.RegisterPreloadScriptFromPath("/preload.js"); // Leading "/" is important

// Create main window here
```

:::tip
Most IgniteView methods involving a path parameter require it to be relative to the web root.  
For example, `/hello.txt` will resolve to:

For vite projects: `src-vite/public/hello.txt`  
For raw HTML projects: `wwwroot/hello.txt`

:::

## Dynamic Preload Scripts

You can also load preload scripts from a simple JavaScript string:

```csharp title="Program.cs"
app.RegisterPreloadScriptFromString("alert('Dynamic preload script works!')");
```

Or with `JSFunctionCall`:

```csharp title="Program.cs"
app.RegisterPreloadScriptFromString(new JSFunctionCall("alert", "Dynamic preload script works!"));
```