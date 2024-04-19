//
//  WindowManagement.h
//  IgniteViewMac
//
//  Created by SamarthCat on 24/03/2023.
//

#ifndef WindowManagement_h
#define WindowManagement_h

//Check If Text Should Be White
bool Whiten(int c){
    if (c/3 < 128){
        return  true;
    }
    return false;
}

void ApplyColors(NSWindow *window, NSColor *value, bool white) {
    window.titlebarAppearsTransparent = true;
    window.backgroundColor = value;
    if (white){
        window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantDark];
    }
    else {
        window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantLight];
    }
}

EXPORT
int InitWindow(int r, int g, int b) {
    bool white = Whiten(r + g + b);
    float d = 255;
    NSColor* value = [NSColor colorWithSRGBRed:r/d green:g/d blue:b/d alpha:1.0f];
    
    for (id window in NSApplication.sharedApplication.windows) {
        ApplyColors(window, value, white);
    }

    
    return 0;
}

EXPORT
void UpdateTitle(char* title){
    NSMenu* mainMenu = [[NSApplication sharedApplication] mainMenu];
    NSMenu* appMenu = [[mainMenu itemAtIndex:0] submenu];
    [appMenu setTitle:[[NSString alloc] initWithUTF8String:title]];
}

EXPORT
void UpdateIcon(char* path){
    NSString* spath = [[NSString alloc] initWithUTF8String: path];
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile: spath];
    [NSApp setApplicationIconImage:icon];
}

#endif /* WindowManagement_h */
