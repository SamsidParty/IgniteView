if (!window._localStorageCache) {
    window._localStorageCache = {};
}

window._localStorage = {
    getItem: (item) => {
        return window._localStorageCache[item] || null;
    },
    setItem: (item, value) => {
        window._localStorageCache[item] = value;
        window.igniteView?.commandBridge?.igniteview_localstorage_set(item, value.toString());
        return value;
    },
    removeItem: (item) => {
        var value = getItem(item);
        delete window._localStorageCache[item];
        window.igniteView?.commandBridge?.igniteview_localstorage_remove(item);
        return value;
    },
    clear: () => {
        window._localStorageCache = {};
        window.igniteView?.commandBridge?.igniteview_localstorage_clear();
    },
    hydrate: async () => {
        window._localStorageCache = await window.igniteView?.commandBridge?.igniteview_localstorage_getAllItems();
        return window._localStorageCache;
    }
}

try {
    Object.defineProperty(window, "localStorage", { value: window._localStorage })
}
catch {}