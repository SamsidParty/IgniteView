window.igniteView.commandBridge = {}
window.igniteView.commandQueue = {}

window.igniteView.commandQueue.add = (commandId, resolve) => {
    // Add the command to the command queue, C# will call this function
    window.igniteView.commandQueue[commandId] = (result) => {
        console.log("[COMMAND BRIDGE] Received result for command " + commandId);
        resolve(JSON.parse(result));
        window.igniteView.commandQueue[commandId] = undefined;
    }
}

window.igniteView.commandBridge.invoke = (command, param) => {
    // Build the command string in format "command:id;param_json"
    var commandId = crypto.randomUUID();
    var commandString = `${command}:${commandId};`;

    console.log("[COMMAND BRIDGE] Sending command of type " + command + " with id " + commandId);

    // Send the command to C#, differs per platform
    if (!!window.saucer) { // Desktop with saucer webview
        return new Promise(async (resolve, reject) => {
            window.igniteView.commandQueue.add(commandId, resolve);
            await window.saucer.exposed.igniteview_commandbridge(commandString);
        });
    }
}