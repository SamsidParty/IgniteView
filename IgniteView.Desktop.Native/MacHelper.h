#pragma once

#define EXPORT extern "C"

EXPORT void MacEnableAcrylic(void* webview, void* window);
EXPORT bool MacIsDark();
EXPORT void MacDirectResize(void* window, int width, int height);
