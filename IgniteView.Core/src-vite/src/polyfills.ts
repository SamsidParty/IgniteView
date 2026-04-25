import { callDynamicCommand } from "./commandBridge";

export type IgniteViewCommandResult<TResult = unknown> = Promise<TResult> | undefined;

declare global {
    interface Element {
        webkitRequestFullscreen?: Element["requestFullscreen"];
        mozRequestFullScreen?: Element["requestFullscreen"];
        msRequestFullscreen?: Element["requestFullscreen"];
    }

    interface Document {
        webkitExitFullscreen?: Document["exitFullscreen"];
        mozCancelFullScreen?: Document["exitFullscreen"];
        msExitFullscreen?: Document["exitFullscreen"];
    }
}

window.open = ((url?: string | URL): ReturnType<Window["open"]> => {
    return callDynamicCommand("igniteview_window_open", url) as unknown as ReturnType<Window["open"]>;
}) as Window["open"];

window.close = ((windowId?: number) => {
    return callDynamicCommand("igniteview_window_close", typeof windowId === "number" ? windowId : -1);
}) as unknown as Window["close"];

window.hide = (windowId?: number) => {
    return callDynamicCommand("igniteview_window_hide", typeof windowId === "number" ? windowId : -1);
};

window.toggleMaximize = () => {
    return callDynamicCommand("igniteview_window_toggle_maximize");
};

window.igniteView.enterFullscreen = () => callDynamicCommand("igniteview_window_enter_fullscreen");
window.igniteView.exitFullscreen = () => callDynamicCommand("igniteview_window_exit_fullscreen");
window.igniteView.toggleFullscreen = () => callDynamicCommand("igniteview_window_toggle_fullscreen");
window.igniteView.isFullscreen = () => callDynamicCommand<boolean>("igniteview_window_is_fullscreen");

(function patchFullscreenAPI(): void {
    let currentFullscreenElement: Element | null = null;

    function dispatchChange(target: EventTarget): void {
        try {
            target.dispatchEvent(new Event("fullscreenchange", { bubbles: true }));
        }
        catch {}
    }

    if (typeof Element !== "undefined" && Element.prototype) {
        Element.prototype.requestFullscreen = function requestFullscreen(): Promise<void> {
            currentFullscreenElement = this;

            return Promise.resolve(window.igniteView.enterFullscreen()).then(() => {
                Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => currentFullscreenElement });
                dispatchChange(currentFullscreenElement || document);
            });
        };

        Element.prototype.webkitRequestFullscreen = Element.prototype.requestFullscreen;
        Element.prototype.mozRequestFullScreen = Element.prototype.requestFullscreen;
        Element.prototype.msRequestFullscreen = Element.prototype.requestFullscreen;
    }

    document.exitFullscreen = function exitFullscreen(): Promise<void> {
        const previousElement = currentFullscreenElement;
        currentFullscreenElement = null;

        return Promise.resolve(window.igniteView.exitFullscreen()).then(() => {
            Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => null });
            dispatchChange(previousElement || document);
        });
    };

    document.webkitExitFullscreen = document.exitFullscreen;
    document.mozCancelFullScreen = document.exitFullscreen;
    document.msExitFullscreen = document.exitFullscreen;

    Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => currentFullscreenElement });
    Object.defineProperty(document, "fullscreenEnabled", { configurable: true, get: () => true });
})();