// This file is generated, to edit it you must edit IgniteView.InjectedScript/InjectedScript.cs

namespace IgniteView.Core
{
    /// <summary>
    /// Generated file that holds the javascript to inject into the html files
    /// </summary>
    public class InjectedScript
    {
        /// <summary>
        /// The built-in IgniteView preload script that is required for IgniteView to work
        /// </summary>
        public const string ScriptData = "\n\nconsole.log(\"[IgniteView][Preload] Injected\");\nwindow.igniteView = { }\nwindow.igniteView.commandBridge = {}\nwindow.igniteView.commandQueue = {}\n\nwindow.igniteView.commandQueue.add = (commandId, resolve) => {\n    // Add the command to the command queue, C# will call this function\n    window.igniteView.commandQueue[commandId] = (result) => {\n        console.log(\"[COMMAND BRIDGE] Received result for command \" + commandId);\n        resolve(JSON.parse(result));\n    }\n}\n\nwindow.igniteView.commandQueue.resolve = (commandId, result) => { // Called by C#\n    // Resolve the command in the command queue\n    if (!!window.igniteView.commandQueue[commandId]) {\n        window.igniteView.commandQueue[commandId](result);\n        window.igniteView.commandQueue[commandId] = undefined;\n    }\n}\n\nwindow.igniteView.commandBridge.invoke = invoke;\n\nfunction invoke(command) {\n\n    var args = Array.prototype.slice.call(arguments);\n\n    // Build the command string in format \"command:id;param_json\"\n    var commandParamData = {\n        paramList: args.filter((_, i) => i > 0) // Ignore first parameter\n    }\n\n    console.log(commandParamData);\n\n    var commandId = crypto.randomUUID();\n    var commandString = `${command}:${commandId};${JSON.stringify(commandParamData)}`;\n\n    console.log(\"[COMMAND BRIDGE] Sending command of type \" + command + \" with id \" + commandId);\n\n    // Send the command to C#, differs per platform\n    if (!!window.saucer) { // Desktop with saucer webview\n        return new Promise(async (resolve, reject) => {\n            window.igniteView.commandQueue.add(commandId, resolve);\n            await window.saucer.exposed.igniteview_commandbridge(commandString);\n        });\n    }\n}\nwindow.open = (url) => window.igniteView.commandBridge.invoke(\"igniteview_window_open\", url);\nwindow.close = (windowId) => window.igniteView.commandBridge.invoke(\"igniteview_window_close\", windowId || -1);"; // Replaced when running build.py

        /// <summary>
        /// List of scripts that are loaded into the WebView before page load, you must set this before any windows are created
        /// </summary>
        public static List<string> PreloadScripts = new List<string>();

        /// <summary>
        /// Combination of all the preload scripts and the IgniteView script
        /// </summary>
        public static string CombinedScriptData {
            get {
                var combinedScripts = ScriptData;
                PreloadScripts.ForEach(script => combinedScripts += "\n" + script);
                return combinedScripts;
            }
        }
    }
}
