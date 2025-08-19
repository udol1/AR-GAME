package com.google.a.b.a.a.a;

import android.os.IBinder;
import android.os.IInterface;

/* compiled from: IInstallService */
public abstract class b extends com.google.a.a.b implements c {
    public static c b(IBinder iBinder) {
        if (iBinder == null) {
            return null;
        }
        IInterface queryLocalInterface = iBinder.queryLocalInterface("com.google.android.play.core.install.protocol.IInstallService");
        if (queryLocalInterface instanceof c) {
            return (c) queryLocalInterface;
        }
        return new a(iBinder);
    }
}
