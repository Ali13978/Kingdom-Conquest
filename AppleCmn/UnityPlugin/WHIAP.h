//
//  WHIAP.h
//  pocketking
//
//  Created by weihua on 12-12-5.
//  Copyright (c) 2012年 weihua. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

enum WHIAP_RST {
    WHIAP_RST_OK = 0,
    WHIAP_RST_FAIL = -1,
    WHIAP_RST_CANNOTBUY = -100,
    };

@protocol WHIAPDelegate <NSObject>

@required
- (void)onIAPError:(NSString *)err code:(int)code;
- (void)onIAPDone:(NSString *)receipt itemid:(NSString *)itemid transid:(NSString *)transid;

@end

@interface WHIAP : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver> {
}

// 用于通知结果的代理对象
@property(nonatomic, assign) id <WHIAPDelegate> delegate;

// 方法
// 进行购买（如果返回NO，表示上一个购买还没有结束）
- (BOOL)doBuy:(NSString *)productID;
// 恢复之前没有结束的购买
- (BOOL)doRestore;

@end
