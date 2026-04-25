import "./main";

window.igniteView.dragWindow = (event: Event): void => {
    const targetElement = event.target;

    if (!window.saucer || !(targetElement instanceof Element)) {
        return;
    }

    if (!targetElement.hasAttribute("data-webview-drag")) {
        targetElement.setAttribute("data-webview-drag", "");
        targetElement.setAttribute("data-webview-maximize", "double");
    }
};