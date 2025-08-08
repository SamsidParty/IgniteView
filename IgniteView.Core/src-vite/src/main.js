
console.log("[IgniteView][Preload] Injected");
window.igniteView = { }

// Makes it easy for C# code to set a variable on the global scope
window.igniteView.set = (name, value) => {
    window[name] = value;
}

window.igniteView.setLocal = (name, value) => {
    window.igniteView[name] = value;
}