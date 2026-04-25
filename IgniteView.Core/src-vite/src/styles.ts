import { getCommandBridge } from "./commandBridge";

window.igniteView.injectCSS = (css: string): void => {
    const style = document.createElement("style");
    style.textContent = css;
    document.head.append(style);
};

window.igniteView.loadGlobalStyles = async (): Promise<void> => {
    const css = await getCommandBridge().invoke<string>("igniteview_get_global_styles");

    if (typeof css === "string") {
        window.igniteView.injectCSS(css);
    }

    console.log("[IgniteView] Loaded global styles");
};

void window.igniteView.loadGlobalStyles();