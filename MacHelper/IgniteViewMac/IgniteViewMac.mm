//
//  IgniteViewMac.m
//  IgniteViewMac
//
//  Created by SamarthCat on 22/03/2023.
//

#include "IgniteViewMac.h"
#include <stdio.h>

#define EXPORT __attribute__((visibility("default")))

//Heap Objects
PanelDel* panelDel;
NSArray* allowedExtensions;

// Initializer.
__attribute__((constructor))
static void initializer(void) {                             
    panelDel = [[PanelDel alloc] init];
}
 
// Finalizer.
__attribute__((destructor))
static void finalizer(void) {
    printf("[%s] finalizer()\n", __FILE__);
}

EXPORT
void FreePointer(void* address){
    free(address);
}

#include "WindowManagement.h"
#include "FilePopups.h"

char* Test(void){
    return "Test";
}

bool IsDark(void){
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

//Objective-C Stuff I Don't Understand

@implementation PanelDel
    
- (BOOL)panel:(id)sender shouldEnableURL:(nonnull NSURL *)url {
    
    NSString* fileExtension = [url pathExtension];
    if (([fileExtension isEqual: @""]) || ([fileExtension isEqual: @"/"]) || (fileExtension == nil)) {
        return YES;
    }

    NSSet* allowed = [NSSet setWithArray:allowedExtensions];
    return [allowed containsObject:[fileExtension lowercaseString]] || [allowed containsObject:@"*"];

}

@end
