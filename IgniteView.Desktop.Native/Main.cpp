#include <saucer/smartview.hpp>
#include <windows.h>
#include <iostream>
#include <list>

#ifdef _WIN32
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
    _declspec(dllexport) void NewWebWindow() {
        auto window = std::shared_ptr{ App->make<saucer::smartview<>>(saucer::preferences{.application = App}) };
        WindowList.push_back(window);

        window->set_title("Hello World!");

        window->set_url("https://google.com");
        window->show();
    }

    _declspec(dllexport) void CreateApp() {
        App = saucer::application::init({
            .id = "hello-world",
        });
    }

    _declspec(dllexport) void RunApp() {
        App->run();
    }
}

