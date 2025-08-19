package com.google.ar.core;

import android.app.Activity;
import android.content.Context;
import android.util.Log;
import com.google.ar.core.ArCoreApk;
import com.google.ar.core.exceptions.ResourceExhaustedException;
import com.google.ar.core.exceptions.UnavailableApkTooOldException;
import com.google.ar.core.exceptions.UnavailableArcoreNotInstalledException;
import com.google.ar.core.exceptions.UnavailableDeviceNotCompatibleException;
import com.google.ar.core.exceptions.UnavailableSdkTooOldException;
import com.google.ar.core.exceptions.UnavailableUserDeclinedInstallationException;
import java.util.HashMap;
import java.util.Map;

class ArCoreApkJniAdapter {

    /* renamed from: a  reason: collision with root package name */
    private static final Map<Class<? extends Throwable>, Integer> f4a;

    static {
        HashMap hashMap = new HashMap();
        f4a = hashMap;
        hashMap.put(IllegalArgumentException.class, Integer.valueOf(af.ERROR_INVALID_ARGUMENT.E));
        f4a.put(ResourceExhaustedException.class, Integer.valueOf(af.ERROR_RESOURCE_EXHAUSTED.E));
        f4a.put(UnavailableArcoreNotInstalledException.class, Integer.valueOf(af.UNAVAILABLE_ARCORE_NOT_INSTALLED.E));
        f4a.put(UnavailableDeviceNotCompatibleException.class, Integer.valueOf(af.UNAVAILABLE_DEVICE_NOT_COMPATIBLE.E));
        f4a.put(UnavailableApkTooOldException.class, Integer.valueOf(af.UNAVAILABLE_APK_TOO_OLD.E));
        f4a.put(UnavailableSdkTooOldException.class, Integer.valueOf(af.UNAVAILABLE_SDK_TOO_OLD.E));
        f4a.put(UnavailableUserDeclinedInstallationException.class, Integer.valueOf(af.UNAVAILABLE_USER_DECLINED_INSTALLATION.E));
    }

    ArCoreApkJniAdapter() {
    }

    private static int a(Throwable th) {
        Log.e("ARCore-ArCoreApkJniAdapter", "Exception details:", th);
        Class<?> cls = th.getClass();
        if (f4a.containsKey(cls)) {
            return f4a.get(cls).intValue();
        }
        return af.ERROR_FATAL.E;
    }

    static int checkAvailability(Context context) {
        try {
            return ArCoreApk.getInstance().checkAvailability(context).nativeCode;
        } catch (Throwable th) {
            a(th);
            return ArCoreApk.Availability.UNKNOWN_ERROR.nativeCode;
        }
    }

    static int requestInstall(Activity activity, boolean z, int[] iArr) throws UnavailableDeviceNotCompatibleException, UnavailableUserDeclinedInstallationException {
        try {
            iArr[0] = ArCoreApk.getInstance().requestInstall(activity, z).nativeCode;
            return af.SUCCESS.E;
        } catch (Throwable th) {
            return a(th);
        }
    }

    static int requestInstallCustom(Activity activity, boolean z, int i, int i2, int[] iArr) throws UnavailableDeviceNotCompatibleException, UnavailableUserDeclinedInstallationException {
        try {
            iArr[0] = ArCoreApk.getInstance().requestInstall(activity, z, ArCoreApk.InstallBehavior.forNumber(i), ArCoreApk.UserMessageType.forNumber(i2)).nativeCode;
            return af.SUCCESS.E;
        } catch (Throwable th) {
            return a(th);
        }
    }
}
