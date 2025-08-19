package com.google.ar.core;

import android.util.Log;
import java.util.concurrent.atomic.AtomicBoolean;

/* compiled from: InstallServiceImpl */
final class ac implements Runnable {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ AtomicBoolean f9a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ ad f10b;

    ac(ad adVar, AtomicBoolean atomicBoolean) {
        this.f10b = adVar;
        this.f9a = atomicBoolean;
    }

    public final void run() {
        if (!this.f9a.getAndSet(true)) {
            Log.w("ARCore-InstallService", "requestInstall timed out, launching fullscreen.");
            ad adVar = this.f10b;
            v.o(adVar.f11a, adVar.f12b);
        }
    }
}
