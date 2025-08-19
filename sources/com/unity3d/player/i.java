package com.unity3d.player;

import android.os.Build;

public final class i {

    /* renamed from: a  reason: collision with root package name */
    static final boolean f144a = (Build.VERSION.SDK_INT >= 19);

    /* renamed from: b  reason: collision with root package name */
    static final boolean f145b = (Build.VERSION.SDK_INT >= 21);
    static final boolean c = (Build.VERSION.SDK_INT >= 23);
    static final boolean d;
    static final d e = (c ? new g() : null);

    static {
        boolean z = true;
        if (Build.VERSION.SDK_INT < 24) {
            z = false;
        }
        d = z;
    }
}
