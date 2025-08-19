package com.unity3d.player;

import android.app.Activity;
import android.content.Context;
import android.os.Looper;
import com.ttt.ArCoreTest.BuildConfig;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;

class b {

    /* renamed from: a  reason: collision with root package name */
    protected n f139a = null;

    /* renamed from: b  reason: collision with root package name */
    protected e f140b = null;
    protected Context c = null;
    protected String d = null;
    protected String e = BuildConfig.FLAVOR;

    b(String str, e eVar) {
        this.e = str;
        this.f140b = eVar;
    }

    /* access modifiers changed from: protected */
    public void reportError(String str) {
        e eVar = this.f140b;
        if (eVar != null) {
            eVar.reportError(this.e + " Error [" + this.d + "]", str);
            return;
        }
        f.Log(6, this.e + " Error [" + this.d + "]: " + str);
    }

    /* access modifiers changed from: protected */
    public void runOnUiThread(Runnable runnable) {
        Context context = this.c;
        if (context instanceof Activity) {
            ((Activity) context).runOnUiThread(runnable);
            return;
        }
        f.Log(5, "Not running " + this.e + " from an Activity; Ignoring execution request...");
    }

    /* access modifiers changed from: protected */
    public boolean runOnUiThreadWithSync(final Runnable runnable) {
        boolean z = true;
        if (Looper.getMainLooper().getThread() == Thread.currentThread()) {
            runnable.run();
            return true;
        }
        final Semaphore semaphore = new Semaphore(0);
        runOnUiThread(new Runnable() {
            public final void run() {
                try {
                    runnable.run();
                } catch (Exception e) {
                    b bVar = b.this;
                    bVar.reportError("Exception unloading Google VR on UI Thread. " + e.getLocalizedMessage());
                } catch (Throwable th) {
                    semaphore.release();
                    throw th;
                }
                semaphore.release();
            }
        });
        try {
            if (!semaphore.tryAcquire(4, TimeUnit.SECONDS)) {
                reportError("Timeout waiting for vr state change!");
                z = false;
            }
            return z;
        } catch (InterruptedException e2) {
            reportError("Interrupted while trying to acquire sync lock. " + e2.getLocalizedMessage());
            return false;
        }
    }
}
