window.open = (url) => window.igniteView.commandBridge.igniteview_window_open(url);
window.close = (windowId) => window.igniteView.commandBridge.igniteview_window_close(typeof(windowId) == "number" ? windowId : -1);
window.hide = (windowId) => window.igniteView.commandBridge.igniteview_window_hide(typeof(windowId) == "number" ? windowId : -1);
window.toggleMaximize = () => window.igniteView.commandBridge.igniteview_window_toggle_maximize();

// Fullscreen
window.igniteView.enterFullscreen = () => window.igniteView.commandBridge.igniteview_window_enter_fullscreen();
window.igniteView.exitFullscreen = () => window.igniteView.commandBridge.igniteview_window_exit_fullscreen();
window.igniteView.toggleFullscreen = () => window.igniteView.commandBridge.igniteview_window_toggle_fullscreen();
window.igniteView.isFullscreen = () => window.igniteView.commandBridge.igniteview_window_is_fullscreen();

// Monkey patch the standard Fullscreen API so HTML elements calling
// element.requestFullscreen() or document.exitFullscreen() drive the native window.
(function patchFullscreenAPI() {
    var currentFullscreenElement = null;

    function dispatchChange(target) {
        try {
            target.dispatchEvent(new Event("fullscreenchange", { bubbles: true }));
        } catch (_) { /* ignore */ }
    }

    if (typeof Element !== "undefined" && Element.prototype) {
        Element.prototype.requestFullscreen = function () {
            currentFullscreenElement = this;
            return Promise.resolve(window.igniteView.enterFullscreen()).then(() => {
                Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => currentFullscreenElement });
                dispatchChange(currentFullscreenElement || document);
            });
        };
        // Vendor-prefixed aliases
        Element.prototype.webkitRequestFullscreen = Element.prototype.requestFullscreen;
        Element.prototype.mozRequestFullScreen = Element.prototype.requestFullscreen;
        Element.prototype.msRequestFullscreen = Element.prototype.requestFullscreen;
    }

    document.exitFullscreen = function () {
        var previousEl = currentFullscreenElement;
        currentFullscreenElement = null;
        return Promise.resolve(window.igniteView.exitFullscreen()).then(() => {
            Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => null });
            dispatchChange(previousEl || document);
        });
    };
    document.webkitExitFullscreen = document.exitFullscreen;
    document.mozCancelFullScreen = document.exitFullscreen;
    document.msExitFullscreen = document.exitFullscreen;

    Object.defineProperty(document, "fullscreenElement", { configurable: true, get: () => currentFullscreenElement });
    Object.defineProperty(document, "fullscreenEnabled", { configurable: true, get: () => true });
})();