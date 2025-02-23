# Calling C# From JavaScript

## Defining Commands

Much like Tauri, IgniteView allows you to expose functions as "Commands", which can be called by JavaScript code.

To create a command: 
    - Write a *public*, *static* function that contains your C# logic
    - Add this function to a *public* class, we recommend creating a dedicated class for all your commands
    - Attach a `[Command]` attribute to your static method, with the parameter being the name of your function in JavaScript

```csharp title="Commands.cs"
[Command("beep")]
public static void Beep(int times)
{
    for (int i = 0; i < times; i++)
    {
        Console.Beep();
    }
}
```

## Calling Commands

In your JavaScript code, the command is now available to call using the `window.igniteView.commandBridge` object:

```javascript title="Main.js"
igniteView.commandBridge.beep(5); // Beeps 5 times
```

## Getting Return Values

You are able to return data from the commands. Most serializable types are supported, including primitive types and arrays.

```csharp title="Commands.cs"
[Command("getUsername")]
public static string GetUsername()
{
    return Environment.UserName;
}
```

Calling from JavaScript will return a `Promise`, which you will need to await.

```javascript
const username = await igniteView.commandBridge.getUsername();
console.log(username);
```
---- OR ----

```javascript
igniteView.commandBridge.getUsername().then(console.log);
```