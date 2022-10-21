//
//  BLEState.h
//  BLEState
//
//  Created by Ximmerse on 2017/10/25.
//  Copyright © 2017年 Ximmerse. All rights reserved.
//


typedef int(*ble_state_delegate)(int state);

#ifdef __cplusplus
extern "C" {
#endif

    void initBLEState(void);
    void unInitBLEState(void);

#ifdef __cplusplus
}
#endif
