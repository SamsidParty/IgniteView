window.igniteView.injectCSS = (css) => {
    var style = document.createElement('style');
    style.textContent = css;
    document.head.append(style);
}

window.igniteView.loadGlobalStyles = async () => {
    var css = await window.igniteView.commandBridge.invoke("igniteview_get_global_styles");
    window.igniteView.injectCSS(css);
    console.log("[IgniteView] Loaded global styles");
}

window.igniteView.loadGlobalStyles();