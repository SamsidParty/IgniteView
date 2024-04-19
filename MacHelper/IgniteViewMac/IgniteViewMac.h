//
//  IgniteViewMac.h
//  IgniteViewMac
//
//  Created by SamarthCat on 22/03/2023.
//

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

void FreePointer(void* address);

int InitWindow(int r, int g, int b);

void UpdateTitle(char* title);

void UpdateIcon(char* path);

char* SaveFile(char* extension);

char* OpenFolder(bool multiSelection);

char* OpenFile(bool multiSelection, char* extensions);

char* Test(void);

bool IsDark(void);

//Objective-C Stuff

@interface PanelDel : NSObject<NSOpenSavePanelDelegate>

@end
