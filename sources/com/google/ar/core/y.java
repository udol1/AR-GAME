package com.google.ar.core;

import android.content.Context;
import android.os.RemoteException;
import android.util.Log;
import com.google.ar.core.ArCoreApk;

/* compiled from: InstallServiceImpl */
final class y implements Runnable {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ Context f45a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ i f46b;
    final /* synthetic */ v c;

    y(v vVar, Context context, i iVar) {
        this.c = vVar;
        this.f45a = context;
        this.f46b = iVar;
    }

    public final void run() {
        try {
            this.c.c.d(this.f45a.getApplicationInfo().packageName, v.k(), new x(this));
        } catch (RemoteException e) {
            Log.e("ARCore-InstallService", "requestInfo threw", e);
            this.f46b.a(ArCoreApk.Availability.UNKNOWN_ERROR);
        }
    }
}
