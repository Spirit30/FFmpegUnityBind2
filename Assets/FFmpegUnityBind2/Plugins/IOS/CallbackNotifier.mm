//
//  CallbackNotifier.m
//  UnityFramework
//
//  Created by Maxim Botvinev on 13.12.2020.
//

#import <MobileFFmpegConfig.h>
#import <MobileFFmpeg.h>

#include "CallbackNotifier.h"

@implementation CallbackNotifier

- (void)onStart:(long)executionId {
    NSString* messageText = [CallbackNotifier toMessage:@"OnStart" executionId:executionId text:@""];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onLog:(long)executionId message:(NSString*)message {
    NSString* messageText = [CallbackNotifier toMessage:@"OnLog" executionId:executionId text:message];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onWarning:(long)executionId message:(NSString*)message {
    NSString* messageText = [CallbackNotifier toMessage:@"OnWarning" executionId:executionId text:message];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onError:(long)executionId message:(NSString*)message {
    NSString* messageText = [CallbackNotifier toMessage:@"OnError" executionId:executionId text:message];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onSuccess:(long)executionId {
    NSString* messageText = [CallbackNotifier toMessage:@"OnSuccess" executionId:executionId text:@""];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onCanceled:(long)executionId {
    NSString* messageText = [CallbackNotifier toMessage:@"OnCanceled" executionId:executionId text:@""];
    [CallbackNotifier sendCallback:messageText];
}

- (void)onFail:(long)executionId {
    NSString* messageText = [CallbackNotifier toMessage:@"OnFail" executionId:executionId text:@""];
    [CallbackNotifier sendCallback:messageText];
}

- (void)executeCallback:(long)executionId :(int)returnCode {
    if (returnCode == RETURN_CODE_SUCCESS) {
        [self onSuccess:executionId];
    } else if (returnCode == RETURN_CODE_CANCEL) {
        [self onCanceled:executionId];
    } else {
        [self onFail:executionId];
    }
}

- (void)logCallback:(long)executionId :(int)level :(NSString*)message {
    
    dispatch_async(dispatch_get_main_queue(), ^{
        if(level >= AV_LOG_INFO) {
            [self onLog:executionId message:message];
        } else if(level >= AV_LOG_WARNING) {
            [self onWarning:executionId message:message];
        } else {
            [self onError:executionId message:message];
        }
    });
}

+ (NSString*) toMessage:(NSString*)eventType executionId:(long)executionId text:(NSString*)text {
    
    return [NSString stringWithFormat:@"%@|%ld|%@", eventType, executionId, text];
}

+ (void) sendCallback:(NSString*)message {
    UnitySendMessage("FFmpegMobileCallbacksHandler", "OnFFmpegMobileCallback", [message UTF8String]);
}

@end
