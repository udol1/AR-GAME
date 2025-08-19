package com.google.ar.core;

import android.hardware.camera2.CameraCaptureSession;
import android.os.Handler;

/* compiled from: SharedCamera */
final class aj extends CameraCaptureSession.StateCallback {
    public static final /* synthetic */ int d = 0;

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ Handler f19a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ CameraCaptureSession.StateCallback f20b;
    final /* synthetic */ SharedCamera c;

    aj(SharedCamera sharedCamera, Handler handler, CameraCaptureSession.StateCallback stateCallback) {
        this.c = sharedCamera;
        this.f19a = handler;
        this.f20b = stateCallback;
    }

    public final void onActive(CameraCaptureSession cameraCaptureSession) {
        this.f19a.post(new SharedCamera$2$$ExternalSyntheticLambda0(this.f20b, cameraCaptureSession));
        this.c.onCaptureSessionActive(cameraCaptureSession);
    }

    public final void onClosed(CameraCaptureSession cameraCaptureSession) {
        this.f19a.post(new SharedCamera$2$$ExternalSyntheticLambda1(this.f20b, cameraCaptureSession));
        this.c.onCaptureSessionClosed(cameraCaptureSession);
    }

    public final void onConfigureFailed(CameraCaptureSession cameraCaptureSession) {
        this.f19a.post(new SharedCamera$2$$ExternalSyntheticLambda2(this.f20b, cameraCaptureSession));
        this.c.onCaptureSessionConfigureFailed(cameraCaptureSession);
    }

    public final void onConfigured(CameraCaptureSession cameraCaptureSession) {
        ak unused = this.c.sharedCameraInfo;
        this.f19a.post(new SharedCamera$2$$ExternalSyntheticLambda3(this.f20b, cameraCaptureSession));
        this.c.onCaptureSessionConfigured(cameraCaptureSession);
        if (this.c.sharedCameraInfo.b() != null) {
            this.c.setDummyListenerToAvoidImageBufferStarvation();
        }
    }

    public final void onReady(CameraCaptureSession cameraCaptureSession) {
        this.f19a.post(new SharedCamera$2$$ExternalSyntheticLambda4(this.f20b, cameraCaptureSession));
        this.c.onCaptureSessionReady(cameraCaptureSession);
    }
}
