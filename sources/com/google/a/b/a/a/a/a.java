package com.google.a.b.a.a.a;

import android.os.Bundle;
import android.os.IBinder;
import android.os.Parcel;
import android.os.RemoteException;
import com.google.a.a.c;
import java.util.List;

/* compiled from: IInstallService */
public final class a extends com.google.a.a.a implements c {
    a(IBinder iBinder) {
        super(iBinder, "com.google.android.play.core.install.protocol.IInstallService");
    }

    public final void d(String str, Bundle bundle, e eVar) throws RemoteException {
        Parcel a2 = a();
        a2.writeString(str);
        c.b(a2, bundle);
        c.c(a2, eVar);
        c(2, a2);
    }

    public final void e(String str, List<Bundle> list, Bundle bundle, e eVar) throws RemoteException {
        Parcel a2 = a();
        a2.writeString(str);
        a2.writeTypedList(list);
        c.b(a2, bundle);
        c.c(a2, eVar);
        c(1, a2);
    }
}
