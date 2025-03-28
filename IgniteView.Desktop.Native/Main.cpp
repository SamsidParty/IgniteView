#include <saucer/smartview.hpp>
#include <saucer/window.hpp>
#include <iostream>
#include <vector>

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

std::shared_ptr<saucer::application> App;
std::vector<std::shared_ptr<saucer::smartview<saucer::default_serializer>>> WindowList;
std::vector<CommandBridgeCallback> CommandBridgeList;


extern "C" {
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge, const char* preloadScript, const char8_t* path) {
        auto window = std::shared_ptr{ App->make<saucer::smartview<saucer::default_serializer>>(saucer::preferences{
            .application = App,
            .persistent_cookies = true,
                .hardware_acceleration = true,
            .storage_path = path
        }) };
        WindowList.push_back(window);
        CommandBridgeList.push_back(commandBridge);
        int windowIndex = WindowList.size() - 1;

        #if __APPLE__
        MacEnableAcrylic(window->webview::native().webview, window->window::native().window);
        #endif
        
        if (preloadScript != 0) {
            window->inject({
                .code = preloadScript,
                .time = saucer::load_time::creation,
                .permanent = true,
            }); 
        }

        window->set_url(url);
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
        WindowList[index]->webview::native().webview->TrySuspend(nullptr);
        #endif
    }

    EXPORT void CloseWebWindow(int index) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->close();
        WindowList[index].reset();
        WindowList[index] = nullptr;
    }

    EXPORT void ExecuteJavaScriptOnWebWindow(int index, const char* javascriptCode) {
        if (WindowList[index] == nullptr) { return; }

        std::basic_string_view stringView(javascriptCode);
        WindowList[index]->execute(stringView);
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

        WindowList[index]->set_title(title);
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

    EXPORT void SetWebWindowIcon(int index, char8_t* iconPath) {
        if (WindowList[index] == nullptr) { return; }

        saucer::icon icon = saucer::icon::from(iconPath).value();
        WindowList[index]->set_icon(icon);
    }

    EXPORT void SetWebWindowURL(int index, const char* url) {
        if (WindowList[index] == nullptr) { return; }

        WindowList[index]->set_url(url);
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

        auto title = WindowList[index]->title(); // This is freed by the C# code
        auto titlePtr = strdup(title.c_str());
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

    EXPORT void CreateApp(const char* appID) {
        App = saucer::application::init({
            .id = appID,
        });
    }

    EXPORT void RunApp() {
        App->run();
    }

    EXPORT void Free(void* ptr) {
        free(ptr);
    }
}

