#include <saucer/smartview.hpp>
#include <iostream>
#include <list>

#ifdef _WIN32
#include <windows.h>

BOOL WINAPI DllMain(
    HINSTANCE hinstDLL,
    DWORD fdwReason,
    LPVOID lpvReserved) {
#else
int main() {
#endif
  return TRUE;
}

std::shared_ptr<saucer::application> App;
std::list<std::shared_ptr<saucer::smartview<saucer::default_serializer>>> WindowList;

extern "C" {
    _declspec(dllexport) void NewWebWindow(const char* url) {
        auto window = std::shared_ptr{ App->make<saucer::smartview<>>(saucer::preferences{.application = App}) };
        WindowList.push_back(window);

        window->set_title("Hello World!");

        window->set_url(url);
        window->show();
    }

    _declspec(dllexport) void CreateApp(const char* appID) {
        App = saucer::application::init({
            .id = appID,
        });
    }

    _declspec(dllexport) void RunApp() {
        App->run();
    }
}

