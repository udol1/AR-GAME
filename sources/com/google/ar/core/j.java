package com.google.ar.core;

import com.google.ar.core.ArCoreApk;

/* compiled from: ArCoreApkImpl */
final class j implements i {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ k f23a;

    j(k kVar) {
        this.f23a = kVar;
    }

    public final void a(ArCoreApk.Availability availability) {
        synchronized (this.f23a) {
            this.f23a.g = availability;
            this.f23a.h = false;
        }
    }
}
