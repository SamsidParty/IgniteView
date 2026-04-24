#include <saucer/smartview.hpp>
#include <saucer/webview.hpp>
#include <saucer/window.hpp>
#include <iostream>
#include <vector>
#include <filesystem>
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

std::shared_ptr<saucer::application> App;
std::vector<std::shared_ptr<saucer::smartview<saucer::default_serializer>>> WindowList;
std::vector<CommandBridgeCallback> CommandBridgeList;

// Per-window saved state used to restore the window after leaving fullscreen.
struct FullscreenState {
    bool active = false;
    bool was_maximized = false;
    saucer::window_decoration decoration = saucer::window_decoration::full;
    std::pair<int, int> position{0, 0};
    std::pair<int, int> size{0, 0};
};
std::vector<FullscreenState> FullscreenStateList;

static void ensure_fullscreen_slot(int index) {
    if (static_cast<int>(FullscreenStateList.size()) <= index) {
        FullscreenStateList.resize(index + 1);
    }
}


extern "C" {
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge, const char* preloadScript, const char* path) {

        std::string urlToSet(url, strlen(url));
        std::string preloadToRun(preloadScript, strlen(preloadScript));
        auto pathToSet = utf8_to_path(path);

        auto window = std::shared_ptr{ App->make<saucer::smartview<saucer::default_serializer>>(saucer::preferences{
            .application = App,
            .persistent_cookies = true,
                .hardware_acceleration = true,
            .storage_path = pathToSet
        }) };
        WindowList.push_back(window);
        CommandBridgeList.push_back(commandBridge);
        int windowIndex = WindowList.size() - 1;
        ensure_fullscreen_slot(windowIndex);

        #if __APPLE__
        MacEnableAcrylic(window->webview::native().webview, window->window::native().window);
        #endif
        
        if (!preloadToRun.empty()) {
            window->inject({
                .code = preloadToRun,
                .time = saucer::load_time::creation,
                .permanent = true,
            }); 
        }

        window->set_url(urlToSet);
        window->expose("igniteview_commandbridge", [windowIndex](std::string param)
            {
                CommandBridgeList[windowIndex](param.c_str());
            });

        return windowIndex;
    }

    EXPORT void ShowWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->show();

        #ifdef _WIN32
        WindowList[index]->webview::native().controller->put_IsVisible(true);
        SetForegroundWindow(WindowList[index]->window::native().hwnd);
        #endif
    }

    EXPORT void HideWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_minimized(true);

        #ifdef _WIN32
        WindowList[index]->webview::native().controller->put_IsVisible(false);
        ICoreWebView2_3* webView2_3 = static_cast<ICoreWebView2_3*>(WindowList[index]->webview::native().webview);
        webView2_3->TrySuspend(nullptr);
        #endif
    }

    EXPORT void CloseWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->close();
        WindowList[index].reset();
        WindowList[index] = nullptr;
    }

    EXPORT void ExecuteJavaScriptOnWebWindow(int index, char* javascriptCode) {  
        if (WindowList[index] == nullptr) { return; }  
        std::string codeToExecute(javascriptCode, strlen(javascriptCode));
        WindowList[index]->webview::execute(codeToExecute);
    }

    EXPORT void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH) {
        if (WindowList[index] == nullptr) { return; }
        
        #if __APPLE__
        // On macOS, calling set_size will animate the resizing, which we don't want
        // So first use the NSWindow API to instantly resize first
        MacDirectResize(WindowList[index]->window::native().window, w, h);
        #endif
        
        WindowList[index]->set_size(w, h);
        WindowList[index]->set_min_size(minW, minH);
        WindowList[index]->set_max_size(maxW, maxH);

        if (minW == maxW && minH == maxH) { // Detect locked window bounds
            WindowList[index]->set_resizable(false);

            // Don't enforce min and max size when bounds are locked
            WindowList[index]->set_min_size(0, 0);
            WindowList[index]->set_max_size(9999, 9999);
        }
        else {
            WindowList[index]->set_resizable(true);
        }
    }

    EXPORT void SetWebWindowTitle(int index, const char* title) {
        if (WindowList[index] == nullptr) { return; }
        std::string titleToSet(title, strlen(title));
        WindowList[index]->set_title(titleToSet);
    }
    
    EXPORT void SetWebWindowTitleBar(int index, bool visible) {
        if (WindowList[index] == nullptr) { return; }

        if (visible) {
            WindowList[index]->set_decoration(saucer::window_decoration::full);
        }
        else {
            WindowList[index]->set_decoration(saucer::window_decoration::partial);
        }
    }

    EXPORT void SetWebWindowIcon(int index, const char* iconPath) {
        if (WindowList[index] == nullptr) { return; }

        auto strIconPath = utf8_to_path(iconPath);
        saucer::icon icon = saucer::icon::from(strIconPath).value();
        WindowList[index]->set_icon(icon);
    }

    EXPORT void SetWebWindowURL(int index, const char* url) {
        if (WindowList[index] == nullptr) { return; }

        std::string urlToSet(url, strlen(url));
        WindowList[index]->set_url(urlToSet);
    }

    EXPORT void SetWebWindowDark(int index, bool isDark) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_force_dark_mode(isDark);
    }

    EXPORT void SetWebWindowDevToolsEnabled(int index, bool enableDevTools) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_dev_tools(enableDevTools);
    }

    EXPORT const char* GetWebWindowTitle(int index) {
        if (WindowList[index] == nullptr) { return ""; }

        auto title = WindowList[index]->title();
        auto titlePtr = strdup(title.c_str()); // This is freed by the C# code
        return (const char*)titlePtr;
    }

    EXPORT const bool GetWebWindowMaximized(int index) {
        return WindowList[index]->maximized();
    }

    EXPORT const void SetWebWindowMaximized(int index, bool isMaximized) {
        WindowList[index]->set_maximized(isMaximized);
    }

    EXPORT const void* GetWebWindowHandle(int index) {
        if (WindowList[index] == nullptr) { return 0; }

        #ifdef _WIN32
        return WindowList[index]->window::native().hwnd;
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
        auto& window = WindowList[index];

        if (fullscreen == state.active) { return; }

        if (fullscreen) {
            // Save pre-fullscreen state so we can restore to the same monitor/position on exit.
            state.was_maximized = window->maximized();
            state.decoration = window->decoration();
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
                window->set_maximized(false);
            }

            window->set_decoration(saucer::window_decoration::none);
            window->set_position(target.position.first, target.position.second);
            window->set_size(target.size.first, target.size.second);

            state.active = true;
        } else {
            // Restore the original decoration first so the frame sizing applies cleanly.
            window->set_decoration(state.decoration);
            window->set_position(state.position.first, state.position.second);
            window->set_size(state.size.first, state.size.second);
            if (state.was_maximized) {
                window->set_maximized(true);
            }
            state.active = false;
        }
    }

    EXPORT void CreateApp(const char* appID) {
        std::string appIDToSet(appID, strlen(appID));
        App = saucer::application::init({
            .id = appIDToSet,
        });
    }

    EXPORT void RunApp() {
        App->run();
    }

    EXPORT void Free(void* ptr) {
        free(ptr);
    }
}

