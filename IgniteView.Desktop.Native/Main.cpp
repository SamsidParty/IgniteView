#include <saucer/smartview.hpp>
#include <saucer/webview.hpp>
#include <saucer/window.hpp>
#include <iostream>
#include <vector>
#include <filesystem>
#include <memory>
#include <optional>
#include <string>
#include <string_view>

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

std::optional<saucer::application> App;
std::vector<std::unique_ptr<saucer::smartview>> WindowList;
std::vector<CommandBridgeCallback> CommandBridgeList;

// Per-window saved state used to restore the window after leaving fullscreen.
struct FullscreenState {
    bool active = false;
    bool was_maximized = false;
    saucer::window::decoration decoration = saucer::window::decoration::full;
    saucer::position position{0, 0};
    saucer::size size{0, 0};
};
std::vector<FullscreenState> FullscreenStateList;

static void ensure_fullscreen_slot(int index) {
    if (static_cast<int>(FullscreenStateList.size()) <= index) {
        FullscreenStateList.resize(index + 1);
    }
}

static coco::stray wait_for_app_shutdown(saucer::application *app) {
    co_await app->finish();
}


extern "C" {
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge, const char* preloadScript, const char* path) {
        if (!App.has_value()) { return -1; }

        std::string urlToSet(url, strlen(url));
        std::string preloadToRun(preloadScript, strlen(preloadScript));
        auto pathToSet = utf8_to_path(path);

        auto hostWindow = saucer::window::create(std::addressof(App.value()));
        if (!hostWindow.has_value()) { return -1; }

        auto webview = saucer::smartview::create({
            .window = hostWindow.value(),
            .persistent_cookies = true,
            .hardware_acceleration = true,
            .storage_path = pathToSet,
        });
        if (!webview.has_value()) { return -1; }

        auto window = std::make_unique<saucer::smartview>(std::move(webview.value()));
        WindowList.push_back(std::move(window));
        CommandBridgeList.push_back(commandBridge);
        int windowIndex = static_cast<int>(WindowList.size()) - 1;
        ensure_fullscreen_slot(windowIndex);

        #if __APPLE__
        MacEnableAcrylic(WindowList[windowIndex]->native().webview, WindowList[windowIndex]->parent().native().window);
        #endif
        
        if (!preloadToRun.empty()) {
            WindowList[windowIndex]->inject({
                .code = preloadToRun,
                .run_at = saucer::script::time::creation,
                .clearable = false,
            }); 
        }

        WindowList[windowIndex]->set_url(urlToSet);
        WindowList[windowIndex]->expose("igniteview_commandbridge", [windowIndex](std::string param)
            {
                CommandBridgeList[windowIndex](param.c_str());
            });

        return windowIndex;
    }

    EXPORT void ShowWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        auto &hostWindow = WindowList[index]->parent();

        hostWindow.show();

        #ifdef _WIN32
        WindowList[index]->native().controller->put_IsVisible(true);
        SetForegroundWindow(hostWindow.native().hwnd);
        #endif
    }

    EXPORT void HideWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        auto &hostWindow = WindowList[index]->parent();
        hostWindow.set_minimized(true);

        #ifdef _WIN32
        WindowList[index]->native().controller->put_IsVisible(false);
        ICoreWebView2_3* webView2_3 = static_cast<ICoreWebView2_3*>(WindowList[index]->native().webview);
        webView2_3->TrySuspend(nullptr);
        #endif
    }

    EXPORT void CloseWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->parent().close();
        WindowList[index].reset();
    }

    EXPORT void ExecuteJavaScriptOnWebWindow(int index, char* javascriptCode) {  
        if (WindowList[index] == nullptr) { return; }  
        std::string codeToExecute(javascriptCode, strlen(javascriptCode));
        WindowList[index]->webview::execute(codeToExecute);
    }

    EXPORT void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH) {
        if (WindowList[index] == nullptr) { return; }

        auto &hostWindow = WindowList[index]->parent();
        
        #if __APPLE__
        // On macOS, calling set_size will animate the resizing, which we don't want
        // So first use the NSWindow API to instantly resize first
        MacDirectResize(hostWindow.native().window, w, h);
        #endif
        
        hostWindow.set_size({.w = w, .h = h});
        hostWindow.set_min_size({.w = minW, .h = minH});
        hostWindow.set_max_size({.w = maxW, .h = maxH});

        if (minW == maxW && minH == maxH) { // Detect locked window bounds
            hostWindow.set_resizable(false);

            // Don't enforce min and max size when bounds are locked
            hostWindow.set_min_size({.w = 0, .h = 0});
            hostWindow.set_max_size({.w = 9999, .h = 9999});
        }
        else {
            hostWindow.set_resizable(true);
        }
    }

    EXPORT void SetWebWindowTitle(int index, const char* title) {
        if (WindowList[index] == nullptr) { return; }
        std::string titleToSet(title, strlen(title));
        WindowList[index]->parent().set_title(titleToSet);
    }
    
    EXPORT void SetWebWindowTitleBar(int index, bool visible) {
        if (WindowList[index] == nullptr) { return; }

        auto &hostWindow = WindowList[index]->parent();

        if (visible) {
            hostWindow.set_decorations(saucer::window::decoration::full);
        }
        else {
            hostWindow.set_decorations(saucer::window::decoration::partial);
        }
    }

    EXPORT void SetWebWindowIcon(int index, const char* iconPath) {
        if (WindowList[index] == nullptr) { return; }

        auto strIconPath = utf8_to_path(iconPath);
        saucer::icon icon = saucer::icon::from(strIconPath).value();
        WindowList[index]->parent().set_icon(icon);
    }

    EXPORT void SetWebWindowURL(int index, const char* url) {
        if (WindowList[index] == nullptr) { return; }

        std::string urlToSet(url, strlen(url));
        WindowList[index]->set_url(urlToSet);
    }

    EXPORT void SetWebWindowDark(int index, bool isDark) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_force_dark(isDark);
    }

    EXPORT void SetWebWindowDevToolsEnabled(int index, bool enableDevTools) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_dev_tools(enableDevTools);
    }

    EXPORT const char* GetWebWindowTitle(int index) {
        if (WindowList[index] == nullptr) { return ""; }

        auto title = WindowList[index]->parent().title();
        auto titlePtr = strdup(title.c_str()); // This is freed by the C# code
        return (const char*)titlePtr;
    }

    EXPORT const bool GetWebWindowMaximized(int index) {
        return WindowList[index] != nullptr && WindowList[index]->parent().maximized();
    }

    EXPORT const void SetWebWindowMaximized(int index, bool isMaximized) {
        if (WindowList[index] == nullptr) { return; }
        WindowList[index]->parent().set_maximized(isMaximized);
    }

    EXPORT const void* GetWebWindowHandle(int index) {
        if (WindowList[index] == nullptr) { return 0; }

        #ifdef _WIN32
        return WindowList[index]->parent().native().hwnd;
        #endif

        return &index;
    }

    EXPORT const bool GetWebWindowFullscreen(int index) {
        if (index < 0 || index >= (int)WindowList.size() || WindowList[index] == nullptr) { return false; }
        ensure_fullscreen_slot(index);
        return FullscreenStateList[index].active;
    }

    EXPORT void SetWebWindowFullscreen(int index, bool fullscreen) {
        if (index < 0 || index >= (int)WindowList.size() || WindowList[index] == nullptr) { return; }
        ensure_fullscreen_slot(index);
        auto& state = FullscreenStateList[index];
        auto& window = WindowList[index]->parent();

        if (fullscreen == state.active) { return; }

        if (fullscreen) {
            // Save pre-fullscreen state so we can restore to the same monitor/position on exit.
            state.was_maximized = window->maximized();
            state.decoration = window.decorations();
            state.position = window->position();
            state.size = window->size();

            // Pick the screen that currently contains the window (falls back to first screen).
            auto current_screen = window->screen();
            saucer::screen target{};
            if (current_screen.has_value()) {
                target = current_screen.value();
            } else {
                auto screens = App->screens();
                if (!screens.empty()) {
                    target = screens.front();
                } else {
                    // No screen information available; bail out rather than produce a zero-sized window.
                    return;
                }
            }

            // Restore from maximized first — toggling decoration while maximized misbehaves on some platforms.
            if (state.was_maximized) {
                window.set_maximized(false);
            }

            window.set_decorations(saucer::window::decoration::none);
            window.set_position(target.position);
            window.set_size(target.size);

            state.active = true;
        } else {
            // Restore the original decoration first so the frame sizing applies cleanly.
            window.set_decorations(state.decoration);
            window.set_position(state.position);
            window.set_size(state.size);
            if (state.was_maximized) {
                window.set_maximized(true);
            }
            state.active = false;
        }
    }

    EXPORT void CreateApp(const char* appID) {
        std::string appIDToSet(appID, strlen(appID));
        auto app = saucer::application::create({
            .id = appIDToSet,
        });
        if (!app.has_value()) { return; }

        App.emplace(std::move(app.value()));
    }

    EXPORT void RunApp() {
        if (!App.has_value()) { return; }
        App->run(wait_for_app_shutdown);
    }

    EXPORT void Free(void* ptr) {
        free(ptr);
    }
}

