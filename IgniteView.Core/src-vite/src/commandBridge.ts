type IgniteViewCommandArgument = unknown;
type IgniteViewCommandResult<TResult = unknown> = Promise<TResult> | string | undefined;
type IgniteViewHostCommandResult<TResult = unknown> = Promise<TResult> | undefined;
type IgniteViewCommandResolver<TResult = unknown> = (result: TResult) => void;

interface IgniteViewCommandParameterData {
    paramList: IgniteViewCommandArgument[];
}

interface IgniteViewCommandQueue {
    add(commandId: string, resolve: IgniteViewCommandResolver): void;
    resolve(commandId: string, result: unknown): void;
    [commandId: string]: unknown;
}

interface IgniteViewCommandBridge {
    invoke<TResult = unknown>(command: string, ...args: IgniteViewCommandArgument[]): IgniteViewCommandResult<TResult>;
    build(): Promise<void>;
    fillCommandList(commandList: string[]): void;
    [commandName: string]: unknown;
}

interface IgniteViewRuntimeCommandState {
    commandQueue: IgniteViewCommandQueue;
}

interface IgniteViewSaucerHost {
    exposed: {
        igniteview_commandbridge(commandString: string): Promise<void> | void;
    };
}

interface IgniteViewWebViewHost {
    postMessage(message: string): Promise<void> | void;
}

type IgniteViewDynamicCommand<TResult = unknown> = (...args: IgniteViewCommandArgument[]) => IgniteViewHostCommandResult<TResult>;

declare global {
    interface Window {
        saucer?: IgniteViewSaucerHost;
        chrome?: {
            webview?: IgniteViewWebViewHost;
            [key: string]: unknown;
        };
    }
}

window.igniteView.commandBridge = {};
(window.igniteView as unknown as IgniteViewRuntimeCommandState).commandQueue = {} as IgniteViewCommandQueue;

const commandBridge = getCommandBridge();
const commandQueue = getCommandQueue();

export function getCommandBridge(): IgniteViewCommandBridge {
    return window.igniteView.commandBridge as unknown as IgniteViewCommandBridge;
}

export function getDynamicCommand<TResult = unknown>(commandName: string): IgniteViewDynamicCommand<TResult> | undefined {
    const command = getCommandBridge()[commandName];
    return typeof command === "function" ? command as IgniteViewDynamicCommand<TResult> : undefined;
}

export function callDynamicCommand<TResult = unknown>(commandName: string, ...args: IgniteViewCommandArgument[]): IgniteViewHostCommandResult<TResult> {
    return getDynamicCommand<TResult>(commandName)?.(...args);
}

function getCommandQueue(): IgniteViewCommandQueue {
    return (window.igniteView as unknown as IgniteViewRuntimeCommandState).commandQueue;
}

commandQueue.add = (commandId: string, resolve: IgniteViewCommandResolver): void => {
    commandQueue[commandId] = (result: unknown): void => {
        resolve(result);
    };
};

commandQueue.resolve = (commandId: string, result: unknown): void => {
    const resolver = commandQueue[commandId] as IgniteViewCommandResolver | undefined;

    if (resolver) {
        resolver(result);
        commandQueue[commandId] = undefined;
    }
};

commandBridge.invoke = invoke;

commandBridge.build = async (): Promise<void> => {
    const commandList = await invoke<string[]>("igniteview_list_commands");
    commandBridge.fillCommandList(Array.isArray(commandList) ? commandList : []);
};

commandBridge.fillCommandList = (commandList: string[]): void => {
    commandList.forEach((command) => {
        const commandName = command.startsWith("streamedCommand/")
            ? command.replace("streamedCommand/", "")
            : command;

        commandBridge[commandName] = (...args: IgniteViewCommandArgument[]) => invoke(command, ...args);
    });
};

function invoke<TResult = unknown>(command: string, ...args: IgniteViewCommandArgument[]): IgniteViewCommandResult<TResult> {
    let commandName = command;
    let isStreamed = false;

    if (commandName.startsWith("streamedCommand/")) {
        isStreamed = true;
        commandName = commandName.replace("streamedCommand/", "");
    }

    const commandParamData: IgniteViewCommandParameterData = {
        paramList: args,
    };

    for (let index = 0; index < commandParamData.paramList.length; index++) {
        const param = commandParamData.paramList[index];

        if (param instanceof Blob) {
            const blobId = "blob_" + crypto.randomUUID();
            commandParamData.paramList[index] = blobId;

            void fetch((window.igniteView.resolverURL as string) + `/blobParameterUpload?blobID=${blobId}`, {
                method: "POST",
                body: param,
            });
        }
    }

    let paramDataString: string;

    try {
        paramDataString = JSON.stringify(commandParamData);
    }
    catch {
        paramDataString = `{"paramList":[]}`;
    }

    const commandId = crypto.randomUUID();
    const commandString = `${commandName}:${commandId};${paramDataString}`;

    if (isStreamed) {
        return (window.igniteView.resolverURL as string) + "/streamedCommand?" + encodeURIComponent(commandString);
    }

    if (window.saucer) {
        return new Promise<TResult>(async (resolve) => {
            commandQueue.add(commandId, resolve as IgniteViewCommandResolver);
            await window.saucer?.exposed.igniteview_commandbridge(commandString);
        });
    }

    if (window.chrome?.webview) {
        return new Promise<TResult>(async (resolve) => {
            commandQueue.add(commandId, resolve as IgniteViewCommandResolver);
            await window.chrome?.webview?.postMessage(commandString);
        });
    }

    return undefined;
}

void getCommandBridge().build();