package com.google.ar.core;

import android.hardware.camera2.CameraDevice;

public final /* synthetic */ class SharedCamera$1$$ExternalSyntheticLambda0 implements Runnable {
    public final /* synthetic */ CameraDevice.StateCallback f$0;
    public final /* synthetic */ CameraDevice f$1;

    public /* synthetic */ SharedCamera$1$$ExternalSyntheticLambda0(CameraDevice.StateCallback stateCallback, CameraDevice cameraDevice) {
        this.f$0 = stateCallback;
        this.f$1 = cameraDevice;
    }

    public final void run() {
        CameraDevice.StateCallback stateCallback = this.f$0;
        CameraDevice cameraDevice = this.f$1;
        int i = ai.d;
        stateCallback.onClosed(cameraDevice);
    }
}
