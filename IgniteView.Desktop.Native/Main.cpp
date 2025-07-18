#include <saucer/smartview.hpp>
#include <saucer/webview.hpp>
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
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge, const char* preloadScript, const char* path) {

        std::string urlToSet(url, strlen(url));
        std::string preloadToRun(preloadScript, strlen(preloadScript));
        std::string pathToSet(path, strlen(path));

        auto window = std::shared_ptr{ App->make<saucer::smartview<saucer::default_serializer>>(saucer::preferences{
            .application = App,
            .persistent_cookies = true,
                .hardware_acceleration = true,
            .storage_path = pathToSet
        }) };
        WindowList.push_back(window);
        CommandBridgeList.push_back(commandBridge);
        int windowIndex = WindowList.size() - 1;

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

        std::string strIconPath(iconPath, strlen(iconPath));
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

