#include "webview/webview.h"

#include <iostream>

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

extern "C" {
    _declspec(dllexport) void InitWebWindow() {
        try {
            webview::webview w(false, nullptr);
            w.set_title("Basic Example");
            w.set_size(480, 320, WEBVIEW_HINT_NONE);
            w.set_html("Hello, World");
            w.run();
        }
        catch (const webview::exception& e) {
            std::cerr << e.what() << '\n';
        }
    }
}

