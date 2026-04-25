#include "MacHelper.h"

#include <Foundation/Foundation.h>
#include <AppKit/AppKit.h>
#include <WebKit/WebKit.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

static NSString* IgniteViewAcrylicBackgroundIdentifier = @"IgniteViewAcrylicBackground";

EXPORT void MacSetAcrylic(void* webviewHandle, void* windowHandle, bool enabled) {
    auto* webview = static_cast<WKWebView*>(webviewHandle);
    auto* window = static_cast<NSWindow*>(windowHandle);

    window.appearance = enabled ? [NSAppearance appearanceNamed:NSAppearanceNameVibrantLight] : nil;
    window.titlebarAppearsTransparent = enabled;
    [webview setValue:[NSNumber numberWithBool:enabled] forKey:@"drawsTransparentBackground"];

    Class vibrantClass = NSClassFromString(@"NSVisualEffectView");
    if (enabled && vibrantClass) {
        for (NSView* subview in window.contentView.subviews) {
            if ([subview.identifier isEqualToString:IgniteViewAcrylicBackgroundIdentifier]) {
                return;
            }
        }

        NSVisualEffectView* vibrant = [[vibrantClass alloc] initWithFrame:window.contentView.bounds];
        vibrant.identifier = IgniteViewAcrylicBackgroundIdentifier;
        [vibrant setAutoresizingMask:NSViewWidthSizable | NSViewHeightSizable];
        [vibrant setBlendingMode:NSVisualEffectBlendingModeBehindWindow];
        [window.contentView addSubview:vibrant positioned:NSWindowBelow relativeTo:nil];
    } else if (!enabled) {
        for (NSView* subview in window.contentView.subviews) {
            if ([subview.identifier isEqualToString:IgniteViewAcrylicBackgroundIdentifier]) {
                [subview removeFromSuperview];
            }
        }
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