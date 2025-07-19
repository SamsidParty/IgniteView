window.igniteView.sharedContext = {};

window.igniteView.sharedContext.getItem = async (key) => {
    return await window.igniteView.commandBridge.invoke("igniteview_get_context_value", key);
}

window.igniteView.sharedContext.setItem = async (key, value) => {
    await window.igniteView.commandBridge.invoke("igniteview_set_context_value", key, value);
}