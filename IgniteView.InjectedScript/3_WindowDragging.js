window.igniteView.dragWindow = (e) => {
    var targetElement = e.target;

    if (!!window.saucer) { // Desktop with saucer webview https://saucer.app/docs/frameless#move--resize
        if (!targetElement.hasAttribute("data-webview-drag")) {
            targetElement.setAttribute("data-webview-drag", "");
        }
    }
}