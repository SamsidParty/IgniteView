#pragma once

#define EXPORT extern "C"

EXPORT void MacSetAcrylic(void* webview, void* window, bool enabled);
EXPORT bool MacIsDark();
EXPORT void MacDirectResize(void* window, int width, int height);
