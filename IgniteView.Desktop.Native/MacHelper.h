#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

#define EXPORT extern "C"

EXPORT void MacEnableAcrylic() {
    NSWindow* window = [[NSApplication sharedApplication] mainWindow];
    window.titlebarAppearsTransparent = true;
}

EXPORT void MacSetDark(bool isDark) {
    
    MacEnableAcrylic();
    
    NSWindow* window = [[NSApplication sharedApplication] mainWindow];
    
    if (isDark) {
        window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantDark];
    }
    else {
        window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantLight];
    }
}

EXPORT bool MacIsDark() {
    if (__builtin_available(macOS 10.14, *))
    {
        NSAppearance* appearance = NSAppearance.currentDrawingAppearance;
        NSAppearanceName basicAppearance = [appearance bestMatchFromAppearancesWithNames:@[
            NSAppearanceNameAqua,
            NSAppearanceNameDarkAqua
        ]];
        return [basicAppearance isEqualToString:NSAppearanceNameDarkAqua];
    }
    return false;
}
