//
//  CallbackNotifier.h
//  UnityFramework
//
//  Created by Maxim Botvinev on 13.12.2020.
//

#ifndef CallbackNotifier_h
#define CallbackNotifier_h

#import <Foundation/Foundation.h>

@interface CallbackNotifier : NSObject

- (void)onStart:(long)executionId;
- (void)onLog:(long)executionId message:(NSString*)message;
- (void)onWarning:(long)executionId message:(NSString*)message;
- (void)onError:(long)executionId message:(NSString*)message;
- (void)onSuccess:(long)executionId;
- (void)onCanceled:(long)executionId;
- (void)onFail:(long)executionId;

- (void)executeCallback:(long)executionId :(int)returnCode;
- (void)logCallback:(long)executionId :(int)level :(NSString*)message;

@end

#endif /* CallbackNotifier_h */
