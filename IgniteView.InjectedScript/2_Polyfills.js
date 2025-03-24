window.open = (url) => window.igniteView.commandBridge.invoke("igniteview_window_open", url);
window.close = (windowId) => window.igniteView.commandBridge.invoke("igniteview_window_close", windowId || -1);
window.hide = (windowId) => window.igniteView.commandBridge.invoke("igniteview_window_hide", windowId || -1);