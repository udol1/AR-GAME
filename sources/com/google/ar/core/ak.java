package com.google.ar.core;

import android.graphics.SurfaceTexture;
import android.hardware.camera2.CameraDevice;
import android.view.Surface;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/* compiled from: SharedCamera */
final class ak {

    /* renamed from: a  reason: collision with root package name */
    private CameraDevice f21a = null;

    /* renamed from: b  reason: collision with root package name */
    private final Map<String, List<Surface>> f22b = new HashMap();
    private SurfaceTexture c = null;
    private Surface d = null;

    private ak() {
    }

    public final SurfaceTexture a() {
        return this.c;
    }

    public final CameraDevice b() {
        return this.f21a;
    }

    public final Surface c() {
        return this.d;
    }

    public final void d(CameraDevice cameraDevice) {
        this.f21a = cameraDevice;
    }

    public final void e(String str, List<Surface> list) {
        this.f22b.put(str, list);
    }

    public final void f(Surface surface) {
        this.d = surface;
    }

    public final void g(SurfaceTexture surfaceTexture) {
        this.c = surfaceTexture;
    }

    /* synthetic */ ak(byte[] bArr) {
    }
}
