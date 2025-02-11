#include <saucer/smartview.hpp>
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

int main() {
  return 0;
}
#endif

typedef void(__stdcall* CommandBridgeCallback)(const wchar_t*);

std::shared_ptr<saucer::application> App;
std::vector<std::shared_ptr<saucer::smartview<saucer::default_serializer>>> WindowList;
std::vector<CommandBridgeCallback> CommandBridgeList;


extern "C" {
    EXPORT int NewWebWindow(const char* url, CommandBridgeCallback commandBridge) {
        auto window = std::shared_ptr{ App->make<saucer::smartview<saucer::default_serializer>>(saucer::preferences{.application = App}) };
        WindowList.push_back(window);
        CommandBridgeList.push_back(commandBridge);
        int windowIndex = WindowList.size() - 1;

        window->set_url(url);
        window->expose("igniteview_commandbridge", [windowIndex](std::wstring param)
            {
                wchar_t* paramPtr = wcsdup(param.c_str()); // This is freed by the C# code
                CommandBridgeList[windowIndex](paramPtr);
            });

        window->show();

        return windowIndex;
    }

    EXPORT void ShowWebWindow(int index) {
        WindowList[index]->show();
    }

    EXPORT void ExecuteJavaScriptOnWebWindow(int index, const char* javascriptCode) {
        std::basic_string_view stringView(javascriptCode);
        WindowList[index]->execute(stringView);
    }

    EXPORT void SetWebWindowBounds(int index, int w, int h, int minW, int minH, int maxW, int maxH) {
        WindowList[index]->set_size(w, h);
        WindowList[index]->set_min_size(minW, minH);
        WindowList[index]->set_max_size(maxW, maxH);

        if (minW == maxW && minH == maxH) { // Detect locked window bounds
            WindowList[index]->set_resizable(false);
        }
        else {
            WindowList[index]->set_resizable(true);
        }
    }

    EXPORT void SetWebWindowTitle(int index, const char* title) {
        WindowList[index]->set_title(title);
    }

    EXPORT void SetWebWindowDark(int index, bool isDark) {
        WindowList[index]->set_force_dark_mode(isDark);
    }

    EXPORT void SetWebWindowDevToolsEnabled(int index, bool enableDevTools) {
        WindowList[index]->set_dev_tools(enableDevTools);
    }

    EXPORT const char* GetWebWindowTitle(int index) {
        auto title = WindowList[index]->title(); // This is freed by the C# code
        auto titlePtr = strdup(title.c_str());
        return (const char*)titlePtr;
    }

    EXPORT const void* GetWebWindowHandle(int index) {
        #ifdef _WIN32
        return WindowList[index]->window::native().hwnd;
        #endif

        return 0;
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

