package com.google.ar.core;

import android.hardware.camera2.CameraDevice;
import android.os.Handler;

/* compiled from: SharedCamera */
final class ai extends CameraDevice.StateCallback {
    public static final /* synthetic */ int d = 0;

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ Handler f17a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ CameraDevice.StateCallback f18b;
    final /* synthetic */ SharedCamera c;

    ai(SharedCamera sharedCamera, Handler handler, CameraDevice.StateCallback stateCallback) {
        this.c = sharedCamera;
        this.f17a = handler;
        this.f18b = stateCallback;
    }

    public final void onClosed(CameraDevice cameraDevice) {
        this.f17a.post(new SharedCamera$1$$ExternalSyntheticLambda0(this.f18b, cameraDevice));
        this.c.onDeviceClosed(cameraDevice);
    }

    public final void onDisconnected(CameraDevice cameraDevice) {
        this.f17a.post(new SharedCamera$1$$ExternalSyntheticLambda1(this.f18b, cameraDevice));
        this.c.onDeviceDisconnected(cameraDevice);
    }

    public final void onError(CameraDevice cameraDevice, int i) {
        this.f17a.post(new SharedCamera$1$$ExternalSyntheticLambda3(this.f18b, cameraDevice, i));
        this.c.close();
    }

    public final void onOpened(CameraDevice cameraDevice) {
        this.c.sharedCameraInfo.d(cameraDevice);
        this.f17a.post(new SharedCamera$1$$ExternalSyntheticLambda2(this.f18b, cameraDevice));
        this.c.onDeviceOpened(cameraDevice);
        this.c.sharedCameraInfo.g(this.c.getGpuSurfaceTexture());
        this.c.sharedCameraInfo.f(this.c.getGpuSurface());
    }
}
