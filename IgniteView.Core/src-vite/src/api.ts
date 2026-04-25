import type { IgniteViewCommandResult } from "./polyfills";
import type { IgniteViewReactAdapter, IgniteViewReactHelpers } from "./reactHelpers";
import type { IgniteViewSharedContext } from "./sharedContext";

export interface IgniteViewApi {
    resolverURL?: string;
    commandBridge: Record<string, unknown>;
    set<TValue = unknown>(name: string, value: TValue): void;
    setLocal<TValue = unknown>(name: string, value: TValue): void;
    sharedContext: IgniteViewSharedContext;
    injectCSS(css: string): void;
    loadGlobalStyles(): Promise<void>;
    enterFullscreen(): IgniteViewCommandResult;
    exitFullscreen(): IgniteViewCommandResult;
    toggleFullscreen(): IgniteViewCommandResult;
    isFullscreen(): IgniteViewCommandResult<boolean>;
    dragWindow(event: Event): void;
    withReact(react: IgniteViewReactAdapter): IgniteViewReactHelpers;
    [name: string]: unknown;
}

declare global {
    interface Window {
        igniteView: IgniteViewApi;
        open(url?: string | URL): IgniteViewCommandResult;
        close(windowId?: number): IgniteViewCommandResult;
        hide(windowId?: number): IgniteViewCommandResult;
        toggleMaximize(): IgniteViewCommandResult;
    }
}