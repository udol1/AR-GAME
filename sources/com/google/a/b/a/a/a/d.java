package com.google.a.b.a.a.a;

import android.os.Bundle;
import android.os.Parcel;
import android.os.RemoteException;
import com.google.a.a.b;
import com.google.a.a.c;

/* compiled from: IInstallServiceCallback */
public abstract class d extends b implements e {
    public d() {
        super("com.google.android.play.core.install.protocol.IInstallServiceCallback");
    }

    /* access modifiers changed from: protected */
    public final boolean a(int i, Parcel parcel) throws RemoteException {
        if (i == 1) {
            c((Bundle) c.a(parcel, Bundle.CREATOR));
        } else if (i == 2) {
            b((Bundle) c.a(parcel, Bundle.CREATOR));
        } else if (i != 3) {
            return false;
        } else {
            Bundle bundle = (Bundle) c.a(parcel, Bundle.CREATOR);
        }
        return true;
    }
}
