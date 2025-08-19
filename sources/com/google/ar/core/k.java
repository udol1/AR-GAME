package com.google.ar.core;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.Intent;
import android.content.IntentSender;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.SystemClock;
import android.util.Log;
import com.google.ar.core.ArCoreApk;
import com.google.ar.core.exceptions.FatalException;
import com.google.ar.core.exceptions.UnavailableDeviceNotCompatibleException;
import com.google.ar.core.exceptions.UnavailableUserDeclinedInstallationException;

/* compiled from: ArCoreApkImpl */
final class k extends ArCoreApk {
    private static final k c = new k();

    /* renamed from: a  reason: collision with root package name */
    Exception f24a;

    /* renamed from: b  reason: collision with root package name */
    boolean f25b = true;
    private boolean d;
    private int e;
    private long f;
    /* access modifiers changed from: private */
    public ArCoreApk.Availability g;
    /* access modifiers changed from: private */
    public boolean h;
    private v i;
    private boolean j;
    private boolean k;
    private int l;

    k() {
    }

    public static k a() {
        return c;
    }

    private static int g(Context context) {
        try {
            PackageInfo packageInfo = context.getPackageManager().getPackageInfo("com.google.ar.core", 4);
            int i2 = packageInfo.versionCode;
            if (i2 != 0) {
                return i2;
            }
            if (packageInfo.services == null || packageInfo.services.length == 0) {
                return -1;
            }
            return 0;
        } catch (PackageManager.NameNotFoundException unused) {
            return -1;
        }
    }

    private final synchronized void h(Context context) {
        if (!this.j) {
            PackageManager packageManager = context.getPackageManager();
            String packageName = context.getPackageName();
            try {
                Bundle bundle = packageManager.getApplicationInfo(packageName, 128).metaData;
                if (bundle.containsKey("com.google.ar.core")) {
                    String string = bundle.getString("com.google.ar.core");
                    string.getClass();
                    this.k = string.equals("required");
                    if (bundle.containsKey("com.google.ar.core.min_apk_version")) {
                        this.l = bundle.getInt("com.google.ar.core.min_apk_version");
                        ActivityInfo[] activityInfoArr = packageManager.getPackageInfo(packageName, 1).activities;
                        String canonicalName = InstallActivity.class.getCanonicalName();
                        for (ActivityInfo activityInfo : activityInfoArr) {
                            if (canonicalName.equals(activityInfo.name)) {
                                this.j = true;
                                return;
                            }
                        }
                        String valueOf = String.valueOf(canonicalName);
                        throw new FatalException(valueOf.length() != 0 ? "Application manifest must contain activity ".concat(valueOf) : new String("Application manifest must contain activity "));
                    }
                    throw new FatalException("Application manifest must contain meta-data com.google.ar.core.min_apk_version");
                }
                throw new FatalException("Application manifest must contain meta-data com.google.ar.core");
            } catch (PackageManager.NameNotFoundException e2) {
                throw new FatalException("Could not load application package metadata", e2);
            } catch (PackageManager.NameNotFoundException e3) {
                throw new FatalException("Could not load application package info", e3);
            }
        }
    }

    private static boolean i() {
        return Build.VERSION.SDK_INT >= 24;
    }

    private final boolean j(Context context) {
        h(context);
        return this.k;
    }

    private static final ArCoreApk.InstallStatus k(Activity activity) throws UnavailableDeviceNotCompatibleException, UnavailableUserDeclinedInstallationException {
        PendingIntent a2 = ah.a(activity);
        if (a2 != null) {
            try {
                Log.i("ARCore-ArCoreApk", "Starting setup activity");
                activity.startIntentSender(a2.getIntentSender(), (Intent) null, 0, 0, 0);
                return ArCoreApk.InstallStatus.INSTALL_REQUESTED;
            } catch (IntentSender.SendIntentException | RuntimeException e2) {
                Log.w("ARCore-ArCoreApk", "Setup activity launch failed", e2);
            }
        }
        return ArCoreApk.InstallStatus.INSTALLED;
    }

    /* access modifiers changed from: package-private */
    public final synchronized v b(Context context) {
        if (this.i == null) {
            v vVar = new v((byte[]) null);
            vVar.a(context.getApplicationContext());
            this.i = vVar;
        }
        return this.i;
    }

    /* access modifiers changed from: package-private */
    public final synchronized void c() {
        if (this.f24a == null) {
            this.e = 0;
        }
        this.d = false;
        v vVar = this.i;
        if (vVar != null) {
            vVar.c();
            this.i = null;
        }
    }

    /* JADX WARNING: Missing exception handler attribute for start block: B:13:0x001e */
    /* Code decompiled incorrectly, please refer to instructions dump. */
    public final com.google.ar.core.ArCoreApk.Availability checkAvailability(android.content.Context r4) {
        /*
            r3 = this;
            boolean r0 = i()
            if (r0 != 0) goto L_0x0009
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNSUPPORTED_DEVICE_NOT_CAPABLE
            return r4
        L_0x0009:
            boolean r0 = r3.d(r4)     // Catch:{ FatalException -> 0x0083 }
            if (r0 == 0) goto L_0x0024
            r3.c()     // Catch:{ FatalException -> 0x0083 }
            android.app.PendingIntent r4 = com.google.ar.core.ah.a(r4)     // Catch:{ UnavailableDeviceNotCompatibleException -> 0x0021, UnavailableUserDeclinedInstallationException | RuntimeException -> 0x001e }
            if (r4 == 0) goto L_0x001b
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.SUPPORTED_APK_TOO_OLD     // Catch:{ UnavailableDeviceNotCompatibleException -> 0x0021, UnavailableUserDeclinedInstallationException | RuntimeException -> 0x001e }
            goto L_0x0023
        L_0x001b:
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.SUPPORTED_INSTALLED     // Catch:{ UnavailableDeviceNotCompatibleException -> 0x0021, UnavailableUserDeclinedInstallationException | RuntimeException -> 0x001e }
            goto L_0x0023
        L_0x001e:
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNKNOWN_ERROR     // Catch:{ FatalException -> 0x0083 }
            goto L_0x0023
        L_0x0021:
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNSUPPORTED_DEVICE_NOT_CAPABLE     // Catch:{ FatalException -> 0x0083 }
        L_0x0023:
            return r4
        L_0x0024:
            monitor-enter(r3)
            com.google.ar.core.ArCoreApk$Availability r0 = r3.g     // Catch:{ all -> 0x0080 }
            if (r0 == 0) goto L_0x002f
            boolean r0 = r0.isUnknown()     // Catch:{ all -> 0x0080 }
            if (r0 == 0) goto L_0x0067
        L_0x002f:
            boolean r0 = r3.h     // Catch:{ all -> 0x0080 }
            if (r0 != 0) goto L_0x0067
            r0 = 1
            r3.h = r0     // Catch:{ all -> 0x0080 }
            com.google.ar.core.j r0 = new com.google.ar.core.j     // Catch:{ all -> 0x0080 }
            r0.<init>(r3)     // Catch:{ all -> 0x0080 }
            boolean r1 = r3.d(r4)     // Catch:{ all -> 0x0080 }
            if (r1 == 0) goto L_0x0047
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.SUPPORTED_INSTALLED     // Catch:{ all -> 0x0080 }
            r0.a(r4)     // Catch:{ all -> 0x0080 }
            goto L_0x0067
        L_0x0047:
            int r1 = g(r4)     // Catch:{ all -> 0x0080 }
            r2 = -1
            if (r1 == r2) goto L_0x0054
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.SUPPORTED_APK_TOO_OLD     // Catch:{ all -> 0x0080 }
            r0.a(r4)     // Catch:{ all -> 0x0080 }
            goto L_0x0067
        L_0x0054:
            boolean r1 = r3.j(r4)     // Catch:{ all -> 0x0080 }
            if (r1 == 0) goto L_0x0060
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.SUPPORTED_NOT_INSTALLED     // Catch:{ all -> 0x0080 }
            r0.a(r4)     // Catch:{ all -> 0x0080 }
            goto L_0x0067
        L_0x0060:
            com.google.ar.core.v r1 = r3.b(r4)     // Catch:{ all -> 0x0080 }
            r1.b(r4, r0)     // Catch:{ all -> 0x0080 }
        L_0x0067:
            com.google.ar.core.ArCoreApk$Availability r4 = r3.g     // Catch:{ all -> 0x0080 }
            if (r4 == 0) goto L_0x006d
            monitor-exit(r3)     // Catch:{ all -> 0x0080 }
            return r4
        L_0x006d:
            boolean r4 = r3.h     // Catch:{ all -> 0x0080 }
            if (r4 == 0) goto L_0x0075
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNKNOWN_CHECKING     // Catch:{ all -> 0x0080 }
            monitor-exit(r3)     // Catch:{ all -> 0x0080 }
            return r4
        L_0x0075:
            java.lang.String r4 = "ARCore-ArCoreApk"
            java.lang.String r0 = "request not running but result is null?"
            android.util.Log.e(r4, r0)     // Catch:{ all -> 0x0080 }
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNKNOWN_ERROR     // Catch:{ all -> 0x0080 }
            monitor-exit(r3)     // Catch:{ all -> 0x0080 }
            return r4
        L_0x0080:
            r4 = move-exception
            monitor-exit(r3)     // Catch:{ all -> 0x0080 }
            throw r4
        L_0x0083:
            r4 = move-exception
            java.lang.String r0 = "ARCore-ArCoreApk"
            java.lang.String r1 = "Error while checking app details and ARCore status"
            android.util.Log.e(r0, r1, r4)
            com.google.ar.core.ArCoreApk$Availability r4 = com.google.ar.core.ArCoreApk.Availability.UNKNOWN_ERROR
            return r4
        */
        throw new UnsupportedOperationException("Method not decompiled: com.google.ar.core.k.checkAvailability(android.content.Context):com.google.ar.core.ArCoreApk$Availability");
    }

    /* access modifiers changed from: package-private */
    public final boolean d(Context context) {
        h(context);
        return g(context) == 0 || g(context) >= this.l;
    }

    public final ArCoreApk.InstallStatus requestInstall(Activity activity, boolean z) throws UnavailableDeviceNotCompatibleException, UnavailableUserDeclinedInstallationException {
        ArCoreApk.UserMessageType userMessageType;
        ArCoreApk.InstallBehavior installBehavior = j(activity) ? ArCoreApk.InstallBehavior.REQUIRED : ArCoreApk.InstallBehavior.OPTIONAL;
        if (j(activity)) {
            userMessageType = ArCoreApk.UserMessageType.APPLICATION;
        } else {
            userMessageType = ArCoreApk.UserMessageType.USER_ALREADY_INFORMED;
        }
        return requestInstall(activity, z, installBehavior, userMessageType);
    }

    public final ArCoreApk.InstallStatus requestInstall(Activity activity, boolean z, ArCoreApk.InstallBehavior installBehavior, ArCoreApk.UserMessageType userMessageType) throws UnavailableDeviceNotCompatibleException, UnavailableUserDeclinedInstallationException {
        if (!i()) {
            throw new UnavailableDeviceNotCompatibleException();
        } else if (d(activity)) {
            c();
            return k(activity);
        } else if (this.d) {
            return ArCoreApk.InstallStatus.INSTALL_REQUESTED;
        } else {
            Exception exc = this.f24a;
            if (exc != null) {
                if (z) {
                    Log.w("ARCore-ArCoreApk", "Clearing previous failure: ", exc);
                    this.f24a = null;
                } else if (exc instanceof UnavailableDeviceNotCompatibleException) {
                    throw ((UnavailableDeviceNotCompatibleException) exc);
                } else if (exc instanceof UnavailableUserDeclinedInstallationException) {
                    throw ((UnavailableUserDeclinedInstallationException) exc);
                } else if (exc instanceof RuntimeException) {
                    throw ((RuntimeException) exc);
                } else {
                    throw new RuntimeException("Unexpected exception type", exc);
                }
            }
            long uptimeMillis = SystemClock.uptimeMillis();
            if (uptimeMillis - this.f > 5000) {
                this.e = 0;
            }
            int i2 = this.e + 1;
            this.e = i2;
            this.f = uptimeMillis;
            if (i2 <= 2) {
                try {
                    activity.startActivity(new Intent(activity, InstallActivity.class).putExtra("message", userMessageType).putExtra("behavior", installBehavior));
                    this.d = true;
                    return ArCoreApk.InstallStatus.INSTALL_REQUESTED;
                } catch (ActivityNotFoundException e2) {
                    throw new FatalException("Failed to launch InstallActivity", e2);
                }
            } else {
                throw new FatalException("Requesting ARCore installation too rapidly.");
            }
        }
    }
}
