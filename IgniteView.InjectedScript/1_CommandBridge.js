window.igniteView.commandBridge = {}
window.igniteView.commandQueue = {}

window.igniteView.commandQueue.add = (commandId, resolve) => {
    // Add the command to the command queue, C# will call this function
    window.igniteView.commandQueue[commandId] = (result) => {
        resolve(result);
    }
}

window.igniteView.commandQueue.resolve = (commandId, result) => { // Called by C#
    // Resolve the command in the command queue
    if (!!window.igniteView.commandQueue[commandId]) {
        window.igniteView.commandQueue[commandId](result);
        window.igniteView.commandQueue[commandId] = undefined;
    }
}

window.igniteView.commandBridge.invoke = invoke;

// Finds all available commands and adds them into the command bridge
window.igniteView.commandBridge.build = async () => {
    var commandList = await invoke("igniteview_list_commands");
    window.igniteView.commandBridge.fillCommandList(commandList);
}

window.igniteView.commandBridge.fillCommandList = (commandList) => {
    commandList.forEach((command) => {
        window.igniteView.commandBridge[command] = function(...args) {
            return invoke(command, ...args)
        };
    })
}


function invoke(command) {

    var args = Array.prototype.slice.call(arguments);

    // Build the command string in format "command:id;param_json"
    var commandParamData = {
        paramList: args.filter((_, i) => i > 0) // Ignore first parameter
    }

    try {
        var paramDataString = JSON.stringify(commandParamData);
    }
    catch {
        var paramDataString = `{"paramList":[]}`;
    }

    var commandId = crypto.randomUUID();
    var commandString = `${command}:${commandId};${paramDataString}`;

    // Send the command to C#, differs per platform
    if (!!window.saucer) { // Desktop with saucer webview
        return new Promise(async (resolve, reject) => {
            window.igniteView.commandQueue.add(commandId, resolve);
            await window.saucer.exposed.igniteview_commandbridge(btoa(commandString));
        });
    }
}

window.igniteView.commandBridge.build();