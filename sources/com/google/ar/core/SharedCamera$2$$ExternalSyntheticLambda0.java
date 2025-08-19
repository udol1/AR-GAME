package com.google.ar.core;

import android.hardware.camera2.CameraCaptureSession;

public final /* synthetic */ class SharedCamera$2$$ExternalSyntheticLambda0 implements Runnable {
    public final /* synthetic */ CameraCaptureSession.StateCallback f$0;
    public final /* synthetic */ CameraCaptureSession f$1;

    public /* synthetic */ SharedCamera$2$$ExternalSyntheticLambda0(CameraCaptureSession.StateCallback stateCallback, CameraCaptureSession cameraCaptureSession) {
        this.f$0 = stateCallback;
        this.f$1 = cameraCaptureSession;
    }

    public final void run() {
        CameraCaptureSession.StateCallback stateCallback = this.f$0;
        CameraCaptureSession cameraCaptureSession = this.f$1;
        int i = aj.d;
        stateCallback.onActive(cameraCaptureSession);
    }
}
