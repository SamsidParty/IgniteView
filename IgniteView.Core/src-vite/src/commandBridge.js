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
        if (command.startsWith("streamedCommand/")) {
            window.igniteView.commandBridge[command.replace("streamedCommand/", "")] = function(...args) {
                return invoke(command, ...args)
            };
        }
        else {
            window.igniteView.commandBridge[command] = function(...args) {
                return invoke(command, ...args)
            };
        }
    })
}


function invoke(command) {

    // Handle streamed command
    if (command.startsWith("streamedCommand/")) {
        var isStreamed = true;
        command = command.replace("streamedCommand/", "");
    }

    var args = Array.prototype.slice.call(arguments);

    // Build the command string in format "command:id;param_json"
    var commandParamData = {
        paramList: args.filter((_, i) => i > 0) // Ignore first parameter
    }


    for (let i = 0; i < commandParamData.paramList.length; i++) {
        const param = commandParamData.paramList[i];
        if (param instanceof Blob) {
            var blobId = "blob_" + crypto.randomUUID();
            commandParamData.paramList[i] = blobId; // Replace Blob with its ID

            // Upload the Blob to the server
            fetch(window.igniteView.resolverURL + `/blobParameterUpload?blobID=${blobId}`, {
                method: "POST",
                body: param
            });
        }
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
    if (!!isStreamed) { // Streamed command (universal)
        var commandURL = igniteView.resolverURL + "/streamedCommand?" + encodeURIComponent(commandString);
        return commandURL;
    }
    else if (!!window.saucer) { // Desktop with saucer webview
        return new Promise(async (resolve, reject) => {
            window.igniteView.commandQueue.add(commandId, resolve);
            await window.saucer.exposed.igniteview_commandbridge(commandString);
        });
    }
}

window.igniteView.commandBridge.build();