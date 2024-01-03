//
//  APIApple.m
//  Unity-iPhone
//
//  Created by weihua on 12-12-5.
//
//

#import "APIApple.h"
#import "WHIAP.h"
#import "WHLog.h"
#import "UIDevice+IdentifierAddition.h"
// 这个原本是在unity的iPhone_target_Prefix.pch中定义的
#ifdef __cplusplus
extern "C" {
#endif
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif

char    unity_obj[128];
char    unity_method[128];
void callbacktounity(const char *str) {
    UnitySendMessage(unity_obj, unity_method, str);
}

@interface APIApple() <WHIAPDelegate>
{
    WHIAP *m_iap;
    NSDictionary *m_mapItemId;
    // 用于存储下面解锁条上的文字
    NSString *m_alertAction;
    // 提示字串
    // 之前的购买未结束
    NSString *g_str_previous_buy_not_done;
    // 无法购买
    NSString *g_str_can_not_buy;
}

@end

@implementation APIApple
- (void)setAlertAction:(NSString *)txt
{
    [m_alertAction release];
    m_alertAction   = [txt retain];
}
- (void)doNotification:(int)sec msg:(NSString *)msg sound:(BOOL)bSet
{
    NSDate *date = [NSDate date];
    /*
     NSTimeZone *zone = [NSTimeZone systemTimeZone];
     NSInteger interval = [zone secondsFromGMTForDate: date];
     NSDate *localeDate = [date  dateByAddingTimeInterval: interval+sec];
     */
    NSDate *localeDate = [date  dateByAddingTimeInterval: sec];
    UILocalNotification *localNotif = [[UILocalNotification alloc] init];
    if (localNotif == nil)
        return;
    localNotif.fireDate = localeDate;
    localNotif.timeZone = [NSTimeZone defaultTimeZone];
    
    localNotif.alertBody = msg;
    localNotif.alertAction = m_alertAction; //NSLocalizedString(@"返回帝国", nil);
    
    if( bSet ) {
        localNotif.soundName = UILocalNotificationDefaultSoundName;
    }
    localNotif.applicationIconBadgeNumber = 0;
    
    //NSDictionary *infoDict = [NSDictionary dictionaryWithObject:item.eventName forKey:ToDoItemKey];
    localNotif.userInfo = nil;
    
    [[UIApplication sharedApplication] scheduleLocalNotification:localNotif];
    //[localNotif release];}
}
- (void)cancelAllNotification
{   
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
}

-(NSString *)getLang
{
    NSUserDefaults *defs    = [NSUserDefaults standardUserDefaults];
    NSArray *langs          = [defs objectForKey:@"AppleLanguages"];
    NSString *preferredLang = [langs objectAtIndex:0];
    return preferredLang;
}

static int znum[6] = {50,100,200,300,500,1000};
- (id)init
{
    if( (self=[super init])) {
        // 初始化IAP
        m_iap   = [WHIAP new];
        m_iap.delegate  = self;
        NSMutableArray *keys = [NSMutableArray new];
        NSMutableArray *vals = [NSMutableArray new];

        for(int i=0;i<6;i++) {
            [keys addObject:[NSString stringWithFormat:@"%d", znum[i]]];
            [vals addObject:[NSString stringWithFormat:@"com.AgeofKingdom.diamonds.%d", znum[i]]];
        }
        m_mapItemId = [[NSDictionary alloc] initWithObjects:vals forKeys:keys];
        
        // 获取语言信息
        NSString *lang = [self getLang];
        if( [lang isEqualToString:@"zh-Hans"] ) {
            g_str_previous_buy_not_done = @"之前的购买尚未结束，请耐心等待";
            g_str_can_not_buy           = @"无法购买";
        }
        else {
            g_str_previous_buy_not_done = @"A previous payment is not done yet. Please wait.";
            g_str_can_not_buy           = @"Can not buy!";
        }
        // 默认参数
        m_alertAction   = @"Go";
    }
    return self;
}
- (void)dealloc
{
    // 取消各种回调
    [super dealloc];
}
- (void)mycallbacktounity:(NSString *)str
{
    callbacktounity([str UTF8String]);
}

- (void)onIAPDone:(NSString *)receipt itemid:(NSString *)itemid transid:(NSString *)transid
{
    NSLog(@"onIAPDone receipt:%@,%@,%@", transid, itemid, receipt);
    [self mycallbacktounity:[NSString stringWithFormat:@"buy_rst %@ %@ %@", transid, itemid, receipt] ];
}
- (void)onIAPError:(NSString *)err code:(int)code;
{    
    NSLog(@"onIAPError:%@,%d", err, code);
    switch (code) {
        case WHIAP_RST_CANNOTBUY:
            [self mycallbacktounity:[NSString stringWithFormat:@"buy_err %@", g_str_can_not_buy] ];
            break;
            
        default:
            [self mycallbacktounity:[NSString stringWithFormat:@"buy_err [%d]%@", code, err] ];
            break;
    }
}

// 购买10的倍数个钻石
static NSString * g_cur_id          = nil;
- (void)buy:(int)num accountid:(NSString *)accid serial:(NSString *)cooOrderSerial
{
    
    // 这个是在buy结束后调用的
    // [self mycallbacktounity:[NSString stringWithFormat:@"buy_rst %d)", rst]];
    // 先测试，根据num觉得是哪个id
    g_cur_id    = [m_mapItemId valueForKey:[NSString stringWithFormat:@"%d", num]];
    if( !g_cur_id ) {
        g_cur_id    = @"com.AgeofKingdom.diamonds.60";
    }

    BOOL    rst = [m_iap doBuy:g_cur_id];
    if( !rst ) {
        // 通知购买失败（上一个没结束，需要等待）
        [self mycallbacktounity:[NSString stringWithFormat:@"buy_err %@", g_str_previous_buy_not_done]];
    }
}
- (void)restorebuy
{
    BOOL    rst = [m_iap doRestore];
    if( !rst ) {
        // 通知购买失败（上一个没结束，需要等待）
        [self mycallbacktounity:[NSString stringWithFormat:@"restore_err %d)", -100]];
    }
}

- (void)getMacAddress
{
    NSString *macaddress = [[UIDevice currentDevice] uniqueGlobalDeviceIdentifier];
    if( macaddress ) {
        [self mycallbacktounity:[NSString stringWithFormat:@"uuid %@", macaddress]];
    }
    else {
        [self mycallbacktounity:@"uuid null"];
    }
}

@end

static APIApple* delegateObject = nil;

extern "C" {
    int setcallback_apple(const char *objname, const char *methodname) {
        // 保存回调对象和方法
        strcpy(unity_obj, objname);
        strcpy(unity_method, methodname);
        return  1;
    }
    // 主程序需要在程序启动前先调用这个来初始化
    int init_apple(const char *objname, const char *methodname) {
        if (delegateObject == nil) {
            delegateObject = [[APIApple alloc] init];
            // 
        }
        setcallback_apple(objname, methodname);
        // 返回１表示成功
        return  1;
    }
    static char myparam[4096];
    static char *myptr[16];
    static BOOL g_bNeedSnd = NO;
    // 用空格分隔参数(返回分隔的数量)
    int mysplitparam(const char *param, int max)
    {
        if( max>16 ) {
            max = 16;
        }
        else if( max<1 ) {
            max = 1;
        }
        int l   = strlen(param);
        if( l>=sizeof(myparam) ) {
            // 超界了
            return -1;
        }
        memcpy(myparam, param, l+1);
        int count=0;
        char *last = myparam+l;
        char *ptr=myparam;
        while( count<max ) {    // 原来<=，会导致多解析一个
            // 当前的有效参数记录一下
            myptr[count]    = ptr;
            // 成功了就要+1
            count ++;
            // 最后一个
            if( count>=max ) {
                break;
            }
            // 查找当前参数的结束位置
            char *next  = strchr(ptr, ' ');
            if( !next ) {
                // 没有下一个参数了
                break;
            }
            *next = 0;
            next = next+1;
            if( next>=last ) {
                // 到了最后一个了
                break;
            }
            // 下一个
            ptr = next;
        }
        
        return count;
    }

    //
    int cmd_apple(const char *cmd, const char *param) {
        if( strcmp(cmd, "alertsound")==0 ) {
            // 格式为：1/0  （表示之后的alert是否需要提示音，最开始不设计默认为不需要，即0）
            g_bNeedSnd   = NO;
            if( param[0]=='1' ) {
                g_bNeedSnd    = YES;
            }
            NSLog(@"**** g_bNeedSnd is %d,%s", g_bNeedSnd, param);
        }
        else if( strcmp(cmd, "alertaction")==0 ) {
            // 解锁条上的文字
            [delegateObject setAlertAction:[NSString stringWithUTF8String:param]];
        }
        else if( strcmp(cmd, "alert")==0 ) {
            // 格式为：秒数 字串
            // 解析出时间和和后面的参数
            int num = mysplitparam(param, 2);
            if( num==2 )
            {
                [delegateObject doNotification:atoi(myptr[0]) msg:[NSString stringWithUTF8String:myptr[1]] sound:g_bNeedSnd];
            }
        }
        else if( strcmp(cmd, "cancelalert")==0 ) {
            // 这个一般在程序从后台恢复到前台时调用（我也可以自动调用）
            [delegateObject cancelAllNotification];
        }
        else if( strcmp(cmd,"buy")==0 ) {
            // 充值购买钻石（单位是多少个10）
            // 参数为：数量 账号id 交易单guid
            int num = mysplitparam(param, 3);
            if( num==3 )
            {
                [delegateObject buy:atoi(myptr[0])
                          accountid:[NSString stringWithUTF8String:myptr[1]]
                             serial:[NSString stringWithUTF8String:myptr[2]]
                ];
            }
        }
        else if( strcmp(cmd,"restore")==0 ) {
            // 恢复一下之前没有结束的购买
            [delegateObject restorebuy];
        }
        else if( strcmp(cmd, "getMacAddress")==0 ) {
            [delegateObject getMacAddress];
        }
        else {
            NSLog(@"unknown cmd:%s %s", cmd, param);
        }
        // 返回１表示成功
        return 1;
    }
}