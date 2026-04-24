#include "MacHelper.h"

#include <Foundation/Foundation.h>
#include <AppKit/AppKit.h>
#include <WebKit/WebKit.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

EXPORT void MacEnableAcrylic(void* webviewHandle, void* windowHandle) {
    auto* webview = static_cast<WKWebView*>(webviewHandle);
    auto* window = static_cast<NSWindow*>(windowHandle);

    window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantLight];
    window.titlebarAppearsTransparent = true;
    [webview setValue:[NSNumber numberWithBool:YES] forKey:@"drawsTransparentBackground"];

    Class vibrantClass = NSClassFromString(@"NSVisualEffectView");
    if (vibrantClass) {
        NSVisualEffectView* vibrant = [[vibrantClass alloc] initWithFrame:window.contentView.bounds];
        [vibrant setAutoresizingMask:NSViewWidthSizable | NSViewHeightSizable];
        [vibrant setBlendingMode:NSVisualEffectBlendingModeBehindWindow];
        [window.contentView addSubview:vibrant positioned:NSWindowBelow relativeTo:nil];
    }
}

EXPORT bool MacIsDark() {
    NSAppearance* appearance = NSApp.effectiveAppearance;
    return [appearance bestMatchFromAppearancesWithNames:@[NSAppearanceNameAqua, NSAppearanceNameDarkAqua]] == NSAppearanceNameDarkAqua;
}

EXPORT void MacDirectResize(void* windowHandle, int width, int height) {
    auto* window = static_cast<NSWindow*>(windowHandle);
    NSRect frame = [window frame];

    frame.size.width = width;
    frame.size.height = height;

    [window setFrame:frame display:true animate:false];
}