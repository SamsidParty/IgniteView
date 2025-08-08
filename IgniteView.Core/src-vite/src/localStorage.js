if (!window._localStorage) {
    class VirtualLocalStorage {

        _cache = {};

        constructor() {
            return new Proxy(this, {
                set: function(target, property, value) {
                    target.setItem(property, value);
                    return true; 
                },
                get: function(target, property) {
                    if (property in target) {
                        return target[property];
                    }

                    return target.getItem(property);
                }
            });
        }

        getItem(item) {
            return this._cache[item] || null;
        }

        setItem(item, value) {
            this._cache[item] = value;

            window.igniteView?.commandBridge?.igniteview_localstorage_set(item, value.toString());
            return value;
        }

        removeItem(item) {
            var value = this.getItem(item);
            delete this._cache[item];
            delete this[item];
            window.igniteView?.commandBridge?.igniteview_localstorage_remove(item);
            return value;
        }

        clear() {
            this._cache = {};
            window.igniteView?.commandBridge?.igniteview_localstorage_clear();
            window._localStorage = new VirtualLocalStorage();
            this.hydrate({});
            delete this;
        }

        // Called by C#
        hydrate(values) {
            Object.entries(values).forEach(([key, value]) => this.setItem(key, value));
            console.log("[IgniteView][LocalStorage] Hydrated");
            try {
                Object.defineProperty(window, "localStorage", { value: window._localStorage })
            }
            catch {}
        }
    }

    window._localStorage = new VirtualLocalStorage();
}

try {
    Object.defineProperty(window, "localStorage", { value: window._localStorage })
}
catch {}