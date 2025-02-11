// This file is generated, to edit it you must edit IgniteView.InjectedScript/InjectedScript.cs

namespace IgniteView.Core
{
    /// <summary>
    /// Generated file that holds the javascript to inject into the html files
    /// </summary>
    public class InjectedScript
    {
        public const string ScriptData = "\nwindow.igniteView = { }\nwindow.igniteView.commandBridge = {}\nwindow.igniteView.commandQueue = {}\n\nwindow.igniteView.commandQueue.add = (commandId, resolve) => {\n    // Add the command to the command queue, C# will call this function\n    window.igniteView.commandQueue[commandId] = (result) => {\n        console.log(\"[COMMAND BRIDGE] Received result for command \" + commandId);\n        resolve(JSON.parse(result));\n        window.igniteView.commandQueue[commandId] = undefined;\n    }\n}\n\nwindow.igniteView.commandBridge.invoke = (command, param) => {\n    // Build the command string in format \"command:id;param_json\"\n    var commandId = crypto.randomUUID();\n    var commandString = `${command}:${commandId};`;\n\n    console.log(\"[COMMAND BRIDGE] Sending command of type \" + command + \" with id \" + commandId);\n\n    // Send the command to C#, differs per platform\n    if (!!window.saucer) { // Desktop with saucer webview\n        return new Promise(async (resolve, reject) => {\n            window.igniteView.commandQueue.add(commandId, resolve);\n            await window.saucer.exposed.igniteview_commandbridge(commandString);\n        });\n    }\n}"; // Replaced when running build.py
    }
}
