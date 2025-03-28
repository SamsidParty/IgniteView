window.open = (url) => window.igniteView.commandBridge.igniteview_window_open(url);
window.close = (windowId) => window.igniteView.commandBridge.igniteview_window_close(typeof(windowId) == "number" ? windowId : -1);
window.hide = (windowId) => window.igniteView.commandBridge.igniteview_window_hide(typeof(windowId) == "number" ? windowId : -1);
window.toggleMaximize = () => window.igniteView.commandBridge.igniteview_window_toggle_maximize();