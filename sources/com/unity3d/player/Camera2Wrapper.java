package com.unity3d.player;

import android.content.Context;
import android.graphics.Rect;
import android.hardware.Camera;

public class Camera2Wrapper implements c {

    /* renamed from: a  reason: collision with root package name */
    private Context f55a;

    /* renamed from: b  reason: collision with root package name */
    private a f56b = null;
    private final int c = 100;

    public Camera2Wrapper(Context context) {
        this.f55a = context;
        initCamera2Jni();
    }

    private static int a(float f) {
        return (int) Math.min(Math.max((f * 2000.0f) - 0.0040893555f, -900.0f), 900.0f);
    }

    private final native void deinitCamera2Jni();

    private final native void initCamera2Jni();

    private final native void nativeFrameReady(Object obj, Object obj2, Object obj3, int i, int i2, int i3);

    private final native void nativeSurfaceTextureReady(Object obj);

    public final void a() {
        deinitCamera2Jni();
        closeCamera2();
    }

    public final void a(Object obj) {
        nativeSurfaceTextureReady(obj);
    }

    public final void a(Object obj, Object obj2, Object obj3, int i, int i2, int i3) {
        nativeFrameReady(obj, obj2, obj3, i, i2, i3);
    }

    /* access modifiers changed from: protected */
    public void closeCamera2() {
        a aVar = this.f56b;
        if (aVar != null) {
            aVar.b();
        }
        this.f56b = null;
    }

    /* access modifiers changed from: protected */
    public int getCamera2Count() {
        if (i.f145b) {
            return a.a(this.f55a);
        }
        return 0;
    }

    /* access modifiers changed from: protected */
    public int[] getCamera2Resolutions(int i) {
        if (i.f145b) {
            return a.d(this.f55a, i);
        }
        return null;
    }

    /* access modifiers changed from: protected */
    public int getCamera2SensorOrientation(int i) {
        if (i.f145b) {
            return a.a(this.f55a, i);
        }
        return 0;
    }

    /* access modifiers changed from: protected */
    public Object getCameraFocusArea(float f, float f2) {
        int a2 = a(f);
        int a3 = a(1.0f - f2);
        return new Camera.Area(new Rect(a2 - 100, a3 - 100, a2 + 100, a3 + 100), 1000);
    }

    /* access modifiers changed from: protected */
    public Rect getFrameSizeCamera2() {
        a aVar = this.f56b;
        return aVar != null ? aVar.a() : new Rect();
    }

    /* access modifiers changed from: protected */
    public boolean initializeCamera2(int i, int i2, int i3, int i4, int i5) {
        if (!i.f145b || this.f56b != null || UnityPlayer.currentActivity == null) {
            return false;
        }
        a aVar = new a(this);
        this.f56b = aVar;
        return aVar.a(this.f55a, i, i2, i3, i4, i5);
    }

    /* access modifiers changed from: protected */
    public boolean isCamera2AutoFocusPointSupported(int i) {
        if (i.f145b) {
            return a.c(this.f55a, i);
        }
        return false;
    }

    /* access modifiers changed from: protected */
    public boolean isCamera2FrontFacing(int i) {
        if (i.f145b) {
            return a.b(this.f55a, i);
        }
        return false;
    }

    /* access modifiers changed from: protected */
    public void pauseCamera2() {
        a aVar = this.f56b;
        if (aVar != null) {
            aVar.d();
        }
    }

    /* access modifiers changed from: protected */
    public boolean setAutoFocusPoint(float f, float f2) {
        a aVar;
        if (!i.f145b || (aVar = this.f56b) == null) {
            return false;
        }
        return aVar.a(f, f2);
    }

    /* access modifiers changed from: protected */
    public void startCamera2() {
        a aVar = this.f56b;
        if (aVar != null) {
            aVar.c();
        }
    }

    /* access modifiers changed from: protected */
    public void stopCamera2() {
        a aVar = this.f56b;
        if (aVar != null) {
            aVar.e();
        }
    }
}
