package com.google.a.a;

import android.os.IBinder;
import android.os.IInterface;
import android.os.Parcel;
import android.os.RemoteException;

/* compiled from: BaseProxy */
public class a implements IInterface {

    /* renamed from: a  reason: collision with root package name */
    private final IBinder f2a;

    /* renamed from: b  reason: collision with root package name */
    private final String f3b;

    protected a(IBinder iBinder, String str) {
        this.f2a = iBinder;
        this.f3b = str;
    }

    /* access modifiers changed from: protected */
    public final Parcel a() {
        Parcel obtain = Parcel.obtain();
        obtain.writeInterfaceToken(this.f3b);
        return obtain;
    }

    public final IBinder asBinder() {
        return this.f2a;
    }

    /* access modifiers changed from: protected */
    public final Parcel b(int i, Parcel parcel) throws RemoteException {
        parcel = Parcel.obtain();
        try {
            this.f2a.transact(i, parcel, parcel, 0);
            parcel.readException();
            return parcel;
        } catch (RuntimeException e) {
            throw e;
        } finally {
            parcel.recycle();
        }
    }

    /* access modifiers changed from: protected */
    public final void c(int i, Parcel parcel) throws RemoteException {
        try {
            this.f2a.transact(i, parcel, (Parcel) null, 1);
        } finally {
            parcel.recycle();
        }
    }
}
