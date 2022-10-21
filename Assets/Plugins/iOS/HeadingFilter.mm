#import "HeadingFilter.h"
@implementation HeadingFilter
@end

static CLLocationManager *locationManager = nil;

extern "C" {
    void LocationManagerInit() {
        locationManager = [[CLLocationManager alloc]init];
        locationManager.headingFilter = 0.01;
        [locationManager startUpdatingHeading];
        locationManager.headingOrientation = CLDeviceOrientationLandscapeLeft;
    }
    
    float LocationManagerGetMagneticHeading() {
        return locationManager.heading.magneticHeading;
    }
    
    float LocationManagerGetMagneticHeadingRawX() {
        return locationManager.heading.x;
    }
    
    float LocationManagerGetMagneticHeadingRawY() {
        return locationManager.heading.y;
    }
    
    float LocationManagerGetMagneticHeadingRawZ() {
        return locationManager.heading.z;
    }
}
