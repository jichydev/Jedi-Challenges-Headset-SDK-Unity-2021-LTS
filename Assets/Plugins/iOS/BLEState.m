//
//  BLEState.m
//  BLEState
//
//  Created by Ximmerse on 2017/10/25.
//  Copyright © 2017年 Ximmerse. All rights reserved.
//


#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#import "BLEState.h"



@interface BLEState : NSObject<CBCentralManagerDelegate>
{
    CBCentralManager* mCBCentralManager;
}
-(void)initBLE;
-(void)unInitBLE;
@end

static ble_state_delegate gFunc = NULL;
static BLEState *gBLEState = NULL;


@implementation BLEState
-(void)initBLE
{
    NSDictionary *options = [NSDictionary dictionaryWithObjectsAndKeys:[NSNumber numberWithBool:YES],CBCentralManagerOptionShowPowerAlertKey,nil];
    mCBCentralManager=[[CBCentralManager alloc]initWithDelegate:self queue:nil options:options];
    unInitBLEState();
}
-(void)unInitBLE
{
    mCBCentralManager = NULL;
}
- (void)centralManagerDidUpdateState:(CBCentralManager *)central{
    switch (central.state) {
            
        case CBManagerStatePoweredOff:
            NSLog(@">>>CBCentralManagerStatePoweredOff=========");
            //if(gFunc)
             //   gFunc(0);
            break;
        case CBManagerStatePoweredOn:
            NSLog(@">>>CBCentralManagerStatePoweredOn===========");
           // if(gFunc)
            //    gFunc(1);
            break;
        default:
            break;
    }
}

@end


#ifdef __cplusplus
extern "C" {
#endif
    
    void initBLEState(void){

        if(!gBLEState){
            gBLEState = [[BLEState alloc] init];
            [gBLEState initBLE];
        }
    }
    void unInitBLEState(void){
        if(gBLEState){
            [gBLEState unInitBLE];
            gBLEState = NULL;
        }
        
        gFunc = NULL;
    }
    
#ifdef __cplusplus
}
#endif


