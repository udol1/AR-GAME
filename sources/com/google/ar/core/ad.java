package com.google.ar.core;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.RemoteException;
import android.util.Log;
import java.util.Collections;
import java.util.concurrent.atomic.AtomicBoolean;

/* compiled from: InstallServiceImpl */
final class ad implements Runnable {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ Activity f11a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ t f12b;
    final /* synthetic */ v c;

    ad(v vVar, Activity activity, t tVar) {
        this.c = vVar;
        this.f11a = activity;
        this.f12b = tVar;
    }

    public final void run() {
        try {
            AtomicBoolean atomicBoolean = new AtomicBoolean(false);
            this.c.c.e(this.f11a.getApplicationInfo().packageName, Collections.singletonList(v.k()), new Bundle(), new ab(this, atomicBoolean));
            new Handler().postDelayed(new ac(this, atomicBoolean), 3000);
        } catch (RemoteException e) {
            Log.w("ARCore-InstallService", "requestInstall threw, launching fullscreen.", e);
            v.o(this.f11a, this.f12b);
        }
    }
}
