// This file is generated, to edit it you must edit IgniteView.InjectedScript/InjectedScript.cs
using System.Text;

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
        public const string ScriptData = "\n\nconsole.log(\"[IgniteView][Preload] Injected\");\nwindow.igniteView = { }\n\n// Makes it easy for C# code to set a variable on the global scope\nwindow.igniteView.set = (name, value) => {\n    window[name] = value;\n}\nwindow.igniteView.commandBridge = {}\nwindow.igniteView.commandQueue = {}\n\nwindow.igniteView.commandQueue.add = (commandId, resolve) => {\n    // Add the command to the command queue, C# will call this function\n    window.igniteView.commandQueue[commandId] = (result) => {\n        resolve(result);\n    }\n}\n\nwindow.igniteView.commandQueue.resolve = (commandId, result) => { // Called by C#\n    // Resolve the command in the command queue\n    if (!!window.igniteView.commandQueue[commandId]) {\n        window.igniteView.commandQueue[commandId](result);\n        window.igniteView.commandQueue[commandId] = undefined;\n    }\n}\n\nwindow.igniteView.commandBridge.invoke = invoke;\n\n// Finds all available commands and adds them into the command bridge\nwindow.igniteView.commandBridge.build = async () => {\n    var commandList = await invoke(\"igniteview_list_commands\");\n    window.igniteView.commandBridge.fillCommandList(commandList);\n}\n\nwindow.igniteView.commandBridge.fillCommandList = (commandList) => {\n    commandList.forEach((command) => {\n        window.igniteView.commandBridge[command] = function(...args) {\n            return invoke(command, ...args)\n        };\n    })\n}\n\n\nfunction invoke(command) {\n\n    var args = Array.prototype.slice.call(arguments);\n\n    // Build the command string in format \"command:id;param_json\"\n    var commandParamData = {\n        paramList: args.filter((_, i) => i > 0) // Ignore first parameter\n    }\n\n    var commandId = crypto.randomUUID();\n    var commandString = `${command}:${commandId};${JSON.stringify(commandParamData)}`;\n\n    // Send the command to C#, differs per platform\n    if (!!window.saucer) { // Desktop with saucer webview\n        return new Promise(async (resolve, reject) => {\n            window.igniteView.commandQueue.add(commandId, resolve);\n            await window.saucer.exposed.igniteview_commandbridge(btoa(commandString));\n        });\n    }\n}\n\nwindow.igniteView.commandBridge.build();\nwindow.open = (url) => window.igniteView.commandBridge.invoke(\"igniteview_window_open\", url);\nwindow.close = (windowId) => window.igniteView.commandBridge.invoke(\"igniteview_window_close\", windowId || -1);\nwindow.igniteView.dragWindow = (e) => {\n    var targetElement = e.target;\n\n    if (!!window.saucer) { // Desktop with saucer webview https://saucer.app/docs/frameless#move--resize\n        if (!targetElement.hasAttribute(\"data-webview-drag\")) {\n            targetElement.setAttribute(\"data-webview-drag\", \"\");\n        }\n    }\n}"; // Replaced when running build.py

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

                // Wrap the code in base64, this is because some of the webview implementations don't allow unicode characters
                return "if (!window.igniteView) { eval(atob('" + Convert.ToBase64String(Encoding.UTF8.GetBytes(combinedScripts)) + "')); }";
            }
        }
    }
}
