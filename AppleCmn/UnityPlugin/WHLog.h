//
//  WHLog.h
//  VideoShop
//
//  Created by weihua on 12-11-17.
//
//  用于在release版下不显示log

#ifndef VideoShop_WHLog_h
#define VideoShop_WHLog_h

#ifndef __OPTIMIZE__
#define NSLog(...)  NSLog(__VA_ARGS__)
#else
#define NSLog(...)  {}
#endif

#endif
