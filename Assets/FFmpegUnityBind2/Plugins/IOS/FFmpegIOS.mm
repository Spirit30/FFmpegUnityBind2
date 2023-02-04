//
//  FFmpegIOS.mm
//  UnityFramework
//
//  Created by Maxim Botvinev on 13.12.2020.
//

#import <MobileFFmpegConfig.h>
#import <MobileFFmpeg.h>

#include "CallbackNotifier.h"

// Converts C style string to NSString
NSString* createNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* makeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {

    long execute(char* command) {
        CallbackNotifier *callbackNotifier = [[CallbackNotifier alloc] init];
        
        long executionId = [MobileFFmpeg executeAsync:createNSString(command) withCallback:callbackNotifier];
        
        [MobileFFmpegConfig setLogDelegate:callbackNotifier];
        
        [callbackNotifier onStart:executionId];
        return executionId;
    }

    void cancel(long executionId) {
        
        [MobileFFmpeg cancel:executionId];
    }
}
