//
//  WHIAP.m
//  tinykingdom
//
//  Created by weihua on 12-12-5.
//  Copyright (c) 2012年 weihua. All rights reserved.
//

#import "WHIAP.h"
#import "NSStrinAdditions.h"
#import "WHLog.h"

@implementation WHIAP {
    // 上一次购买是否还未结束
    BOOL        _bIAPStarted;
    // 上一次购买开始的时刻
    time_t      _nLastIAPTime;
}

- (id)init
{
    self    = [super init];
    // 增加内购结果监听
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    return  self;
}
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"got products info:");
    NSArray *myProduct = response.products;
    NSLog(@"非法产品Product ID:%@",response.invalidProductIdentifiers);
    NSLog(@"产品付费数量: %d", [myProduct count]);
    if( [myProduct count]==0 )
    {
        // 返回失败
        [self.delegate onIAPError:@"cannotbuy" code:WHIAP_RST_CANNOTBUY];
        // 标记一下交易结束
        _bIAPStarted    = NO;
        return;
    }
    // 这里应该只有一个
    for(SKProduct *product in myProduct){
        NSLog(@"product info");
        NSLog(@"SKProduct 描述信息%@", [product description]);
        NSLog(@"产品标题 %@" , product.localizedTitle);
        NSLog(@"产品描述信息: %@" , product.localizedDescription);
        NSLog(@"价格: %@" , product.price);
        NSLog(@"Product id: %@" , product.productIdentifier);
        
        SKPayment *payment = [SKPayment paymentWithProduct:product];
        NSLog(@"Sending payment ...");
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    }
}
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error{
    NSLog(@"----request didFailWithError 错误信息:%@", error);
}
- (void)requestDidFinish:(SKRequest *)request
{
    NSLog(@"----------反馈信息结束--------------");
}
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    // 能到这里也说明结束了
    NSLog(@"**** paymentQueueRestoreCompletedTransactionsFinished");
    // 标记一下交易结束
    _bIAPStarted    = NO;
}
- (void)completeTransaction:(SKPaymentTransaction *)transaction
{
    [self recordTransaction:transaction];
    [self finishTransaction:transaction wasSuccessful:YES];
}
- (void)recordTransaction:(SKPaymentTransaction *)transaction
{
    // 告诉代理最终结束的transaction内容
    [self.delegate onIAPDone:[NSString base64StringFromData:transaction.transactionReceipt length:0] itemid:transaction.payment.productIdentifier transid:transaction.transactionIdentifier];
}

//
// removes the transaction from the queue and posts a notification with the transaction result
//
- (void)finishTransaction:(SKPaymentTransaction *)transaction wasSuccessful:(BOOL)wasSuccessful
{
    // remove the transaction from the payment queue.
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    /*
     NSDictionary *userInfo = [NSDictionary dictionaryWithObjectsAndKeys:transaction, @"transaction" , nil];
     if (wasSuccessful)
     {
     NSLog(@"finishTransaction wasSuccessful");
     }
     else
     {
     NSLog(@"finishTransaction FAILED");
     }
     */
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    if( !_bIAPStarted ) {
        // 没有开始交易呢
        return;
    }
    for (SKPaymentTransaction *transaction in transactions)
    {
        NSLog(@"updatedTransactions:%d", transaction.transactionState);
        //[self finishTransaction:transaction wasSuccessful:NO];
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased://交易完成
                NSLog(@"-----交易完成 --------");
                [self completeTransaction:transaction];
                // 标记一下交易结束
                _bIAPStarted    = NO;
                break;
            case SKPaymentTransactionStateFailed://交易失败
                NSLog(@"-----交易失败 --------");
               // [self finishTransaction:(transaction)];
                [self finishTransaction:transaction wasSuccessful:NO];
               // [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
               // [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
                [self.delegate onIAPError:@"SKPaymentTransactionStateFailed" code:WHIAP_RST_FAIL ];
                // 标记一下交易结束
                _bIAPStarted    = NO;
                break;
            case SKPaymentTransactionStateRestored://已经购买过该商品
                NSLog(@"-----恢复之前的结果 --------");
                [self completeTransaction:transaction];
                // 标记一下交易结束
                _bIAPStarted    = NO;
                break;
            case SKPaymentTransactionStatePurchasing:      //商品添加进列表
                NSLog(@"----商品添加进列表----");
                break;
            default:
                // 标记一下交易结束
                _bIAPStarted    = NO;
                 [self finishTransaction:transaction wasSuccessful:NO];
                NSLog(@"paymentQueue----未知状态:%d", transaction.transactionState);
                break;
        }
    }
}
- (void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions)
    {
        NSLog(@"removedTransactions:%d", transaction.transactionState);
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased://交易完成
                NSLog(@"-----交易完成 --------");
                break;
            case SKPaymentTransactionStateFailed://交易失败
                NSLog(@"-----交易失败 --------");
                break;
            case SKPaymentTransactionStateRestored://已经购买过该商品
                NSLog(@"-----商品被恢复 --------");
                break;
            case SKPaymentTransactionStatePurchasing:      //商品添加进列表
                NSLog(@"----商品添加进列表----");
                break;
            default:
                NSLog(@"paymentQueue----未知状态:%d", transaction.transactionState);
                break;
        }
    }
}

- (BOOL)doBuy:(NSString *)productID
{
    if( _bIAPStarted ) {
        // 检查一下是不是时间太长了
        if( time(NULL)-_nLastIAPTime>2*60 ) {
            // 可以认为没反应断线了，就重新开始吧
            _bIAPStarted    = NO;
        }
        else {
            NSLog(@"doBuy: A previous buy is NOT finished!");
            return NO;
        }
    }
    if( [SKPaymentQueue canMakePayments] ) {
        _bIAPStarted    = YES;
        _nLastIAPTime   = time(NULL);
        SKProductsRequest *_productreq  = [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithObject:productID]];
        _productreq.delegate    = self;
        [_productreq start];
         
    }
   return YES;
}
- (BOOL)doRestore
{
    if( _bIAPStarted ) {
        NSLog(@"doRestore: A previous buy is NOT finished!");
        return NO;
    }
    _bIAPStarted    = YES;
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
    return YES;
}

@end
