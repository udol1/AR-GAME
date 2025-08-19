package com.google.ar.core;

import android.os.Bundle;
import android.os.RemoteException;
import android.util.Log;
import com.google.a.b.a.a.a.d;
import com.google.ar.core.exceptions.FatalException;
import java.util.concurrent.atomic.AtomicBoolean;

/* compiled from: InstallServiceImpl */
final class ab extends d {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ AtomicBoolean f7a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ ad f8b;

    ab(ad adVar, AtomicBoolean atomicBoolean) {
        this.f8b = adVar;
        this.f7a = atomicBoolean;
    }

    public final void b(Bundle bundle) throws RemoteException {
    }

    public final void c(Bundle bundle) throws RemoteException {
        if (!this.f7a.getAndSet(true)) {
            int i = bundle.getInt("error.code", -100);
            int i2 = bundle.getInt("install.status", 0);
            if (i2 == 4) {
                this.f8b.f12b.a(u.COMPLETED);
            } else if (i != 0) {
                StringBuilder sb = new StringBuilder(51);
                sb.append("requestInstall = ");
                sb.append(i);
                sb.append(", launching fullscreen.");
                Log.w("ARCore-InstallService", sb.toString());
                ad adVar = this.f8b;
                v.o(adVar.f11a, adVar.f12b);
            } else if (bundle.containsKey("resolution.intent")) {
                ad adVar2 = this.f8b;
                v.p(adVar2.f11a, bundle, adVar2.f12b);
            } else if (i2 != 10) {
                switch (i2) {
                    case 1:
                    case 2:
                    case 3:
                        this.f8b.f12b.a(u.ACCEPTED);
                        return;
                    case 4:
                        this.f8b.f12b.a(u.COMPLETED);
                        return;
                    case 5:
                        this.f8b.f12b.b(new FatalException("Unexpected FAILED install status without error."));
                        return;
                    case 6:
                        this.f8b.f12b.a(u.CANCELLED);
                        return;
                    default:
                        t tVar = this.f8b.f12b;
                        StringBuilder sb2 = new StringBuilder(38);
                        sb2.append("Unexpected install status: ");
                        sb2.append(i2);
                        tVar.b(new FatalException(sb2.toString()));
                        return;
                }
            } else {
                this.f8b.f12b.b(new FatalException("Unexpected REQUIRES_UI_INTENT install status without an intent."));
            }
        }
    }
}
