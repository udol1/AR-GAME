package com.google.ar.core;

import android.hardware.camera2.CameraDevice;

public final /* synthetic */ class SharedCamera$1$$ExternalSyntheticLambda3 implements Runnable {
    public final /* synthetic */ CameraDevice.StateCallback f$0;
    public final /* synthetic */ CameraDevice f$1;
    public final /* synthetic */ int f$2;

    public /* synthetic */ SharedCamera$1$$ExternalSyntheticLambda3(CameraDevice.StateCallback stateCallback, CameraDevice cameraDevice, int i) {
        this.f$0 = stateCallback;
        this.f$1 = cameraDevice;
        this.f$2 = i;
    }

    public final void run() {
        CameraDevice.StateCallback stateCallback = this.f$0;
        CameraDevice cameraDevice = this.f$1;
        int i = this.f$2;
        int i2 = ai.d;
        stateCallback.onError(cameraDevice, i);
    }
}
