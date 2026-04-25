import { getCommandBridge } from "./commandBridge";

export interface IgniteViewSharedContext {
    getItem<TValue = unknown>(key: string): Promise<TValue>;
    setItem<TValue = unknown>(key: string, value: TValue): Promise<void>;
    getAllItems<TValue = unknown>(): Promise<Record<string, TValue>>;
}

window.igniteView.sharedContext = {
    getItem: async <TValue = unknown>(key: string): Promise<TValue> => {
        return await getCommandBridge().invoke<TValue>("igniteview_get_context_value", key) as TValue;
    },
    setItem: async <TValue = unknown>(key: string, value: TValue): Promise<void> => {
        await getCommandBridge().invoke("igniteview_set_context_value", key, value);
    },
    getAllItems: async <TValue = unknown>(): Promise<Record<string, TValue>> => {
        return await getCommandBridge().invoke<Record<string, TValue>>("igniteview_get_all_context_values") as Record<string, TValue>;
    },
};