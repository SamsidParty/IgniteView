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

std::shared_ptr<saucer::application> App;
std::vector<std::shared_ptr<saucer::webview>> WindowList;



extern "C" {
    EXPORT int NewWebWindow(const char* url) {
        auto window = std::shared_ptr{ App->make<saucer::webview>(saucer::preferences{.application = App}) };
        WindowList.push_back(window);

        window->set_url(url);
        window->show();

        return WindowList.size() - 1;
    }

    EXPORT void ShowWebWindow(int index) {
        WindowList[index]->show();
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

    EXPORT const char* GetWebWindowTitle(int index) {
        auto title = WindowList[index]->title();
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

