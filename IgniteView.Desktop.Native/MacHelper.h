#include <Foundation/Foundation.h>
#include <AppKit/AppKit.h>
#include <WebKit/WebKit.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

#define EXPORT extern "C"

EXPORT void MacEnableAcrylic(WKWebView* webview, NSWindow* window) {
    
    window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantLight];
    window.titlebarAppearsTransparent = true;
    [webview setValue:[NSNumber numberWithBool: YES] forKey:@"drawsTransparentBackground"];
    
    Class vibrantClass=NSClassFromString(@"NSVisualEffectView");
    if (vibrantClass)
    {
        NSVisualEffectView *vibrant=[[vibrantClass alloc] initWithFrame:window.contentView.bounds];
        [vibrant setAutoresizingMask:NSViewWidthSizable|NSViewHeightSizable];
        [vibrant setBlendingMode:NSVisualEffectBlendingModeBehindWindow];
        [window.contentView addSubview:vibrant positioned:NSWindowBelow relativeTo:nil];
    }
}

EXPORT bool MacIsDark() {
    NSAppearance* appearance = NSApp.effectiveAppearance;
    NSString* name = appearance.name;
    return [appearance bestMatchFromAppearancesWithNames:@[NSAppearanceNameAqua, NSAppearanceNameDarkAqua]] == NSAppearanceNameDarkAqua;
}

// Resizes the window without animating
EXPORT void MacDirectResize(NSWindow* window, int width, int height) {
    NSRect frame = [window frame];

    frame.size.width = width;
    frame.size.height = height;
    
    [window setFrame:frame display:true animate:false];
}
