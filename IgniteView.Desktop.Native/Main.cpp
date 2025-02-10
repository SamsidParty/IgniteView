#include <saucer/smartview.hpp>
#include <iostream>
#include <vector>

#ifdef _WIN32
#include <windows.h>
#include <saucer/modules/stable/webview2.hpp>

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
    return TRUE;
}

#else
int main() {
  return TRUE;
}
#endif

std::shared_ptr<saucer::application> App;
std::vector<std::shared_ptr<saucer::webview>> WindowList;



extern "C" {
    _declspec(dllexport) int NewWebWindow(const char* url) {
        auto window = std::shared_ptr{ App->make<saucer::webview>(saucer::preferences{.application = App}) };
        WindowList.push_back(window);

        window->set_url(url);
        window->show();

        return WindowList.size() - 1;
    }

    _declspec(dllexport) void ShowWebWindow(int index) {
        WindowList[index]->show();
    }

    _declspec(dllexport) void SetWebWindowTitle(int index, const char* title) {
        WindowList[index]->set_title(title);
    }

    _declspec(dllexport) void SetWebWindowDark(int index, bool isDark) {
        WindowList[index]->set_force_dark_mode(isDark);
    }

    _declspec(dllexport) const char* GetWebWindowTitle(int index) {
        auto title = WindowList[index]->title();
        auto titlePtr = strdup(title.c_str());
        return (const char*)titlePtr;
    }

    _declspec(dllexport) const void* GetWebWindowHandle(int index) {
        #ifdef _WIN32
        return WindowList[index]->window::native().hwnd;
        #endif

        return 0;
    }

    _declspec(dllexport) void CreateApp(const char* appID) {
        App = saucer::application::init({
            .id = appID,
        });
    }

    _declspec(dllexport) void RunApp() {
        App->run();
    }

    _declspec(dllexport) void Free(void* ptr) {
        free(ptr);
    }
}

