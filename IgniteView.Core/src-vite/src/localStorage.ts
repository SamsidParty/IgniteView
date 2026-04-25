import { callDynamicCommand } from "./commandBridge";

interface IgniteViewVirtualLocalStorage {
    getItem(item: string): string | null;
    setItem(item: string, value: string): string;
    removeItem(item: string): string | null;
    clear(): void;
    hydrate(values: Record<string, string>): void;
}

declare global {
    interface Window {
        _localStorage?: IgniteViewVirtualLocalStorage;
    }
}

if (!window._localStorage) {
    class VirtualLocalStorage implements IgniteViewVirtualLocalStorage {
        private _cache: Record<string, string> = {};

        constructor() {
            return new Proxy(this, {
                set(target, property, value): boolean {
                    if (typeof property === "string") {
                        target.setItem(property, String(value));
                        return true;
                    }

                    return Reflect.set(target, property, value);
                },
                get(target, property, receiver) {
                    if (property in target) {
                        return Reflect.get(target, property, receiver);
                    }

                    return typeof property === "string" ? target.getItem(property) : undefined;
                },
            });
        }

        getItem(item: string): string | null {
            return this._cache[item] || null;
        }

        setItem(item: string, value: string): string {
            this._cache[item] = value;

            void callDynamicCommand("igniteview_localstorage_set", item, value.toString());

            return value;
        }

        removeItem(item: string): string | null {
            const value = this.getItem(item);
            delete this._cache[item];
            delete (this as unknown as Record<string, unknown>)[item];

            void callDynamicCommand("igniteview_localstorage_remove", item);

            return value;
        }

        clear(): void {
            this._cache = {};

            void callDynamicCommand("igniteview_localstorage_clear");

            window._localStorage = new VirtualLocalStorage();
            this.hydrate({});
        }

        hydrate(values: Record<string, string>): void {
            Object.entries(values).forEach(([key, value]) => {
                this._cache[key] = value;
            });

            console.log("[IgniteView][LocalStorage] Hydrated");

            try {
                Object.defineProperty(window, "localStorage", { value: window._localStorage });
            }
            catch {}
        }
    }

    window._localStorage = new VirtualLocalStorage();
}

try {
    Object.defineProperty(window, "localStorage", { value: window._localStorage });
}
catch {}