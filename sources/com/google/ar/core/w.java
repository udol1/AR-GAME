package com.google.ar.core;

import android.content.ComponentName;
import android.content.ServiceConnection;
import android.os.IBinder;

/* compiled from: InstallServiceImpl */
final class w implements ServiceConnection {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ v f43a;

    w(v vVar) {
        this.f43a = vVar;
    }

    public final void onServiceConnected(ComponentName componentName, IBinder iBinder) {
        this.f43a.l(iBinder);
    }

    public final void onServiceDisconnected(ComponentName componentName) {
        this.f43a.m();
    }
}
