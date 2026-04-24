#include <saucer/smartview.hpp>
#include <saucer/webview.hpp>
#include <saucer/window.hpp>
#include <iostream>
#include <vector>
#include <filesystem>
#include <string>
#include <string_view>
#include <memory>
#include <optional>
#include <utility>

#include <coco/stray/stray.hpp>

#ifdef _WIN32
#include <windows.h>
#include <saucer/modules/stable/webview2.hpp>

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
    return TRUE;
}

#define EXPORT _declspec(dllexport)

#else

#define EXPORT extern "C"
#define __stdcall

int main() {
  return 0;
}
#endif

#ifdef __APPLE__
#include <saucer/modules/stable/webkit.hpp>
#include "MacHelper.h"
#endif



typedef void(__stdcall *CommandBridgeCallback)(const char*);

// Converts a UTF8 encoded C string coming from the C# side into a std::filesystem::path
static std::filesystem::path utf8_to_path(const char* s) {
    if (s == nullptr) {
        return {};
    }
    std::string_view sv(s);
    return std::filesystem::path(
        reinterpret_cast<const char8_t*>(sv.data()),
        reinterpret_cast<const char8_t*>(sv.data() + sv.size()));
}

// Per-window saved state used to restore the window after leaving fullscreen.
struct WebWindowEntry {
    std::shared_ptr<saucer::window> window;
    std::optional<saucer::smartview> webview;
    CommandBridgeCallback commandBridge{nullptr};
    bool alive{false};
};

// App is heap-allocated because saucer::application is move-only and has
// `operator&` deleted; we need a stable pointer to pass to saucer::window::create.
std::unique_ptr<saucer::application> App;

// Each entry is heap-allocated so references captured by exposed functions remain
// valid even when the vector grows.
std::vector<std::unique_ptr<WebWindowEntry>> WindowList;

static WebWindowEntry* entry_at(int index) {
    if (index < 0 || index >= (int)WindowList.size()) { return nullptr; }
    auto* entry = WindowList[index].get();
    if (entry == nullptr || !entry->alive) { return nullptr; }
    return entry;
}


extern "C" {
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge, const char* preloadScript, const char* path) {

        std::string urlToSet(url, strlen(url));
        std::string preloadToRun(preloadScript, strlen(preloadScript));
        auto pathToSet = utf8_to_path(path);

        auto window_result = saucer::window::create(App.get());
        if (!window_result.has_value()) {
            return -1;
        }
        auto window = std::move(window_result.value());

        saucer::webview::options webview_opts{
            .window = window,
            .persistent_cookies = true,
            .hardware_acceleration = true,
            .storage_path = pathToSet,
        };

        auto webview_result = saucer::smartview::create(webview_opts);
        if (!webview_result.has_value()) {
            return -1;
        }

        auto entry = std::make_unique<WebWindowEntry>();
        entry->window = window;
        entry->webview.emplace(std::move(webview_result.value()));
        entry->commandBridge = commandBridge;
        entry->alive = true;

        WindowList.push_back(std::move(entry));
        int windowIndex = static_cast<int>(WindowList.size()) - 1;
        auto* entryPtr = WindowList[windowIndex].get();

        #if __APPLE__
        MacEnableAcrylic(entryPtr->webview->native().webview, entryPtr->window->native().window);
        #endif

        if (!preloadToRun.empty()) {
            entryPtr->webview->inject({
                .code = preloadToRun,
                .run_at = saucer::script::time::creation,
                .clearable = false,
            });
        }

        entryPtr->webview->set_url(urlToSet);
        entryPtr->webview->expose("igniteview_commandbridge", [windowIndex](std::string param)
            {
                auto* e = entry_at(windowIndex);
                if (e == nullptr || e->commandBridge == nullptr) { return; }
                e->commandBridge(param.c_str());
            });

        return windowIndex;
    }

    EXPORT void ShowWebWindow(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        e->window->show();

        #ifdef _WIN32
        e->webview->native().controller->put_IsVisible(true);
        SetForegroundWindow(e->window->native().hwnd);
        #endif
    }

    EXPORT void HideWebWindow(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        e->window->set_minimized(true);

        #ifdef _WIN32
        e->webview->native().controller->put_IsVisible(false);
        ICoreWebView2_3* webView2_3 = static_cast<ICoreWebView2_3*>(e->webview->native().webview);
        webView2_3->TrySuspend(nullptr);
        #endif
    }

    EXPORT void CloseWebWindow(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        e->window->close();
        e->webview.reset();
        e->window.reset();
        e->alive = false;
    }

    EXPORT void ExecuteJavaScriptOnWebWindow(int index, char* javascriptCode) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }
        std::string codeToExecute(javascriptCode, strlen(javascriptCode));
        // Call webview::execute (cstring_view) directly; smartview::execute is a
        // consteval format_string template and cannot accept a runtime string.
        static_cast<saucer::webview&>(*e->webview).execute(codeToExecute);
    }

    EXPORT void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        #if __APPLE__
        // On macOS, calling set_size will animate the resizing, which we don't want
        // So first use the NSWindow API to instantly resize first
        MacDirectResize(e->window->native().window, w, h);
        #endif

        e->window->set_size(saucer::size{w, h});
        e->window->set_min_size(saucer::size{minW, minH});
        e->window->set_max_size(saucer::size{maxW, maxH});

        if (minW == maxW && minH == maxH) { // Detect locked window bounds
            e->window->set_resizable(false);

            // Don't enforce min and max size when bounds are locked
            e->window->set_min_size(saucer::size{0, 0});
            e->window->set_max_size(saucer::size{9999, 9999});
        }
        else {
            e->window->set_resizable(true);
        }
    }

    EXPORT void SetWebWindowTitle(int index, const char* title) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }
        std::string titleToSet(title, strlen(title));
        e->window->set_title(titleToSet);
    }

    EXPORT void SetWebWindowTitleBar(int index, bool visible) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        if (visible) {
            e->window->set_decorations(saucer::window::decoration::full);
        }
        else {
            e->window->set_decorations(saucer::window::decoration::partial);
        }
    }

    EXPORT void SetWebWindowIcon(int index, const char* iconPath) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        auto strIconPath = utf8_to_path(iconPath);
        auto icon_result = saucer::icon::from(strIconPath);
        if (!icon_result.has_value()) { return; }
        e->window->set_icon(icon_result.value());
    }

    EXPORT void SetWebWindowURL(int index, const char* url) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        std::string urlToSet(url, strlen(url));
        e->webview->set_url(urlToSet);
    }

    EXPORT void SetWebWindowDark(int index, bool isDark) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        // Renamed from `set_force_dark_mode` to `set_force_dark` in saucer v7.
        e->webview->set_force_dark(isDark);
    }

    EXPORT void SetWebWindowDevToolsEnabled(int index, bool enableDevTools) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }

        e->webview->set_dev_tools(enableDevTools);
    }

    EXPORT const char* GetWebWindowTitle(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return ""; }

        auto title = e->window->title();
        auto titlePtr = strdup(title.c_str()); // This is freed by the C# code
        return (const char*)titlePtr;
    }

    EXPORT const bool GetWebWindowMaximized(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return false; }
        return e->window->maximized();
    }

    EXPORT const void SetWebWindowMaximized(int index, bool isMaximized) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }
        e->window->set_maximized(isMaximized);
    }

    EXPORT const void* GetWebWindowHandle(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return 0; }

        #ifdef _WIN32
        return e->window->native().hwnd;
        #else
        // Non-Windows platforms do not yet expose a stable handle through this API.
        return nullptr;
        #endif
    }

    EXPORT const bool GetWebWindowFullscreen(int index) {
        auto* e = entry_at(index);
        if (e == nullptr) { return false; }
        return e->window->fullscreen();
    }

    EXPORT void SetWebWindowFullscreen(int index, bool fullscreen) {
        auto* e = entry_at(index);
        if (e == nullptr) { return; }
        // Use saucer's native fullscreen so the compositor (e.g. KDE Wayland) grants
        // a real fullscreen surface that covers panels/taskbars.
        e->window->set_fullscreen(fullscreen);
    }

    EXPORT void CreateApp(const char* appID) {
        std::string appIDToSet(appID, strlen(appID));
        auto app_result = saucer::application::create({
            .id = appIDToSet,
        });
        if (!app_result.has_value()) {
            return;
        }
        // saucer::application is move-only; heap-allocate so we keep a stable pointer.
        App = std::unique_ptr<saucer::application>(
            new saucer::application(std::move(app_result.value())));
    }

    EXPORT void RunApp() {
        if (!App) { return; }
        // saucer v7+ requires a start coroutine. Windows are already created via
        // NewWebWindow before this call, so the start coroutine just waits for the
        // last window to close (which resolves `app->finish()`).
        App->run([](saucer::application* app) -> coco::stray {
            co_await app->finish();
        });
    }

    EXPORT void Free(void* ptr) {
        free(ptr);
    }
}

