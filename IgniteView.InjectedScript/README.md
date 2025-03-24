# Building The Injected Script
To build the injected script, simply run `build.py`.
All it does is combine all the `.js` files in this directory, and place it into the `InjectedScript.cs` file.
The `InjectedScript.cs` is then copied into `../IgniteView.Core/Types/InjectedScript.cs`.

The JS files in this directory are in the order that they should be added, so `0_xxx.js` will be added before `1_xxx.js`.