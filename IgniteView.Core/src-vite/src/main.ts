import type { IgniteViewApi } from "./api";

console.log("[IgniteView][Preload] Injected");
window.igniteView = {} as IgniteViewApi;

window.igniteView.set = <TValue = unknown>(name: string, value: TValue): void => {
    (window as unknown as Record<string, TValue>)[name] = value;
};

window.igniteView.setLocal = <TValue = unknown>(name: string, value: TValue): void => {
    (window.igniteView as Record<string, unknown>)[name] = value;
};