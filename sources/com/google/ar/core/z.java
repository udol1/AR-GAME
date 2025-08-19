package com.google.ar.core;

import android.content.pm.PackageInstaller;
import android.util.Log;
import java.util.HashMap;
import java.util.Map;

/* compiled from: InstallServiceImpl */
final class z extends PackageInstaller.SessionCallback {

    /* renamed from: a  reason: collision with root package name */
    final Map<Integer, PackageInstaller.SessionInfo> f47a = new HashMap();

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ t f48b;
    final /* synthetic */ v c;

    z(v vVar, t tVar) {
        this.c = vVar;
        this.f48b = tVar;
    }

    public final void onActiveChanged(int i, boolean z) {
    }

    public final void onBadgingChanged(int i) {
    }

    public final void onCreated(int i) {
        this.f47a.put(Integer.valueOf(i), this.c.g.getSessionInfo(i));
    }

    public final void onFinished(int i, boolean z) {
        PackageInstaller.SessionInfo remove = this.f47a.remove(Integer.valueOf(i));
        if (remove != null && "com.google.ar.core".equals(remove.getAppPackageName())) {
            Log.i("ARCore-InstallService", "Detected ARCore install completion");
            this.f48b.a(u.COMPLETED);
        }
    }

    public final void onProgressChanged(int i, float f) {
    }
}
