package com.google.vr.dynamite.client;

import android.os.IBinder;

/* compiled from: ILoadedInstanceCreator */
public final class a extends com.google.a.a.a implements ILoadedInstanceCreator {
    a(IBinder iBinder) {
        super(iBinder, "com.google.vr.dynamite.client.ILoadedInstanceCreator");
    }

    /* JADX WARNING: type inference failed for: r0v2, types: [android.os.IInterface] */
    /* JADX WARNING: Multi-variable type inference failed */
    /* JADX WARNING: Unknown variable types count: 1 */
    /* Code decompiled incorrectly, please refer to instructions dump. */
    public final com.google.vr.dynamite.client.INativeLibraryLoader newNativeLibraryLoader(com.google.vr.dynamite.client.IObjectWrapper r3, com.google.vr.dynamite.client.IObjectWrapper r4) throws android.os.RemoteException {
        /*
            r2 = this;
            android.os.Parcel r0 = r2.a()
            com.google.a.a.c.c(r0, r3)
            com.google.a.a.c.c(r0, r4)
            r3 = 1
            android.os.Parcel r3 = r2.b(r3, r0)
            android.os.IBinder r4 = r3.readStrongBinder()
            if (r4 != 0) goto L_0x0017
            r4 = 0
            goto L_0x002b
        L_0x0017:
            java.lang.String r0 = "com.google.vr.dynamite.client.INativeLibraryLoader"
            android.os.IInterface r0 = r4.queryLocalInterface(r0)
            boolean r1 = r0 instanceof com.google.vr.dynamite.client.INativeLibraryLoader
            if (r1 == 0) goto L_0x0025
            r4 = r0
            com.google.vr.dynamite.client.INativeLibraryLoader r4 = (com.google.vr.dynamite.client.INativeLibraryLoader) r4
            goto L_0x002b
        L_0x0025:
            com.google.vr.dynamite.client.b r0 = new com.google.vr.dynamite.client.b
            r0.<init>(r4)
            r4 = r0
        L_0x002b:
            r3.recycle()
            return r4
        */
        throw new UnsupportedOperationException("Method not decompiled: com.google.vr.dynamite.client.a.newNativeLibraryLoader(com.google.vr.dynamite.client.IObjectWrapper, com.google.vr.dynamite.client.IObjectWrapper):com.google.vr.dynamite.client.INativeLibraryLoader");
    }
}
