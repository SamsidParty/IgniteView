# Calling JavaScript From C# # 

## Defining Functions

You can define a JavaScript function with the usual syntax, but you have to make sure it's accessible in the global scope.
This means you have to add your function to the `window` object.

```javascript
function doStuff(theStuff) {
    console.log(theStuff);
}
window.doStuff = doStuff; // Important
```

## Calling Functions

Now you can simply call the function from your C# code using the `WebWindow.CallFunction()` method:

```csharp
mainWindow.CallFunction("doStuff", "The stuff has been done!");
```

Returning values is not currently supported.