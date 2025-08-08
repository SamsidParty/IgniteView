
console.log("[IgniteView][Preload] Injected");
window.igniteView = { }

// Makes it easy for C# code to set a variable on the global scope
window.igniteView.set = (name, value) => {
    window[name] = value;
}

window.igniteView.setLocal = (name, value) => {
    window.igniteView[name] = value;
}

window.igniteView.loadScript = (url) => {
    const script = document.createElement('script');
    script.src = url;
        
    if (document.head) {
        document.head.appendChild(script);
    } else {
        document.addEventListener('DOMContentLoaded', function() {
            document.head.appendChild(script);
        });
    }
}