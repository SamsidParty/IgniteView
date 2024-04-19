//
//  FilePopups.h
//  IgniteViewMac
//
//  Created by SamarthCat on 24/03/2023.
//

#ifndef FilePopups_h
#define FilePopups_h

EXPORT
char* SaveFile(char* extension){
    
    NSSavePanel* panel = [NSSavePanel savePanel];
    char* __block path = malloc(1024); // Max Path Size
    strcpy(path, "nr"); // Short For Not Returned

    [panel setMessage:@"Save File"];
    [panel setExtensionHidden:NO];
    [panel setCanCreateDirectories:YES];
    [panel setTitle:@"Save File"];
    [panel setNameFieldStringValue:@"file.as"];
    

    [panel beginWithCompletionHandler:^(NSInteger result){
        if (result == NSModalResponseOK) {
            strcpy(path, [[[panel URL] path] UTF8String]);
        }
        else {
            strcpy(path, "null");
        }
    }];
    
    return path;
}

EXPORT
char* OpenFolder(bool multiSelection) {
    NSOpenPanel* panel = [NSOpenPanel openPanel];
    char* __block path = malloc(1024*1024); // 1MB Should Be Enough To Store A Ton Of Paths
    strcpy(path, "nr"); // Short For Not Returned
    
    [panel setMessage:(@"Select Folder")];
    [panel setTitle:(@"Select Folder")];
    [panel setCanChooseFiles:NO];
    [panel setCanChooseDirectories:YES];
    [panel setAllowsMultipleSelection:multiSelection];
    
    [panel beginWithCompletionHandler:^(NSInteger result){
        if (result == NSModalResponseOK) {
            
            NSArray* files = [panel URLs];
            NSString* output = @"";
            
            for (int i = 0; i < files.count; i++){
                NSString* uPath = [files[i] path];
                
                if (i != 0){
                    output = [output stringByAppendingString:@":"];
                }
                output = [output stringByAppendingString:uPath];
            }
            
            strcpy(path, [output UTF8String]);
        }
        else {
            strcpy(path, "null");
        }
    }];
    
    return path;
}

EXPORT
char* OpenFile(bool multiSelection, char* extensions) {
    NSOpenPanel* panel = [NSOpenPanel openPanel];
    char* __block path = malloc(1024*1024); // 1MB Should Be Enough To Store A Ton Of Paths
    strcpy(path, "nr"); // Short For Not Returned
    
    [panel setMessage:(@"Select File")];
    [panel setTitle:(@"Select File")];
    [panel setCanChooseFiles:YES];
    [panel setCanChooseDirectories:NO];
    [panel setAllowsMultipleSelection:multiSelection];
    [panel setExtensionHidden:NO];
    
    //Move extensions into allowedExtensions
    NSString* ext = [NSString stringWithUTF8String: extensions]; //Convert
    allowedExtensions = [ext componentsSeparatedByString:@":seperate:"];
    
    panel.delegate = panelDel;
    
    [panel beginWithCompletionHandler:^(NSInteger result){
        if (result == NSModalResponseOK) {
            
            NSArray* files = [panel URLs];
            NSString* output = @"";
            
            for (int i = 0; i < files.count; i++){
                NSString* uPath = [files[i] path];
                
                if (i != 0){
                    output = [output stringByAppendingString:@":"];
                }
                output = [output stringByAppendingString:uPath];
            }
            
            strcpy(path, [output UTF8String]);
        }
        else {
            strcpy(path, "");
        }
    }];
    
    return path;
}

#endif /* FilePopups_h */
