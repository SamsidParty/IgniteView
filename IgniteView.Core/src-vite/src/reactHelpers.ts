import { getCommandBridge } from "./commandBridge";

export type IgniteViewLoadingState = "none" | "loading" | "success" | "failed";
export type IgniteViewReactSetState<TValue> = (value: TValue) => void;

export interface IgniteViewReactAdapter {
    useState<TValue>(initialValue: TValue): [TValue, IgniteViewReactSetState<TValue>];
}

export interface IgniteViewSharedContextBinding<TValue = unknown> {
    value: TValue | null;
    setValue(value: TValue): Promise<void>;
}

export interface IgniteViewReactHelpers {
    useCommandResult<TResult = unknown>(command: string, ...args: unknown[]): TResult | null;
    useSharedContext<TValue = unknown>(key: string): IgniteViewSharedContextBinding<TValue>;
}

window.igniteView.withReact = (React: IgniteViewReactAdapter): IgniteViewReactHelpers => ({
    useCommandResult<TResult = unknown>(command: string, ...args: unknown[]): TResult | null {
        const [result, setResult] = React.useState<TResult | null>(null);
        const [loadingState, setLoadingState] = React.useState<IgniteViewLoadingState>("none");

        if (loadingState === "none") {
            setLoadingState("loading");
            setTimeout(async () => {
                try {
                    const commandResult = await getCommandBridge().invoke<TResult>(command, ...args);
                    setResult(commandResult as TResult);
                    setLoadingState("success");
                }
                catch {
                    setLoadingState("failed");
                }
            }, 0);
        }

        return result;
    },
    useSharedContext<TValue = unknown>(key: string): IgniteViewSharedContextBinding<TValue> {
        const [result, setResult] = React.useState<TValue | null>(null);
        const [loadingState, setLoadingState] = React.useState<IgniteViewLoadingState>("none");

        if (loadingState === "none") {
            setLoadingState("loading");
            setTimeout(async () => {
                try {
                    setResult(await window.igniteView.sharedContext.getItem<TValue>(key));
                    setLoadingState("success");
                }
                catch {
                    setLoadingState("failed");
                }
            }, 0);
        }

        return {
            value: result,
            setValue: async (value: TValue): Promise<void> => {
                await window.igniteView.sharedContext.setItem(key, value);
                setResult(value);
            },
        };
    },
});