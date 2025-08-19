package com.google.ar.core;

import android.os.Bundle;
import android.os.RemoteException;
import android.util.Log;
import com.google.a.b.a.a.a.d;
import com.google.ar.core.ArCoreApk;

/* compiled from: InstallServiceImpl */
final class x extends d {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ y f44a;

    x(y yVar) {
        this.f44a = yVar;
    }

    public final void b(Bundle bundle) throws RemoteException {
        int i = bundle.getInt("error.code", -100);
        if (i == -5) {
            Log.e("ARCore-InstallService", "The device is not supported.");
            this.f44a.f46b.a(ArCoreApk.Availability.UNSUPPORTED_DEVICE_NOT_CAPABLE);
        } else if (i == -3) {
            Log.e("ARCore-InstallService", "The Google Play application must be updated.");
            this.f44a.f46b.a(ArCoreApk.Availability.UNKNOWN_ERROR);
        } else if (i != 0) {
            StringBuilder sb = new StringBuilder(33);
            sb.append("requestInfo returned: ");
            sb.append(i);
            Log.e("ARCore-InstallService", sb.toString());
            this.f44a.f46b.a(ArCoreApk.Availability.UNKNOWN_ERROR);
        } else {
            this.f44a.f46b.a(ArCoreApk.Availability.SUPPORTED_NOT_INSTALLED);
        }
    }

    public final void c(Bundle bundle) throws RemoteException {
    }
}
