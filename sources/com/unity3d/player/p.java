package com.unity3d.player;

import android.app.Activity;
import android.content.Context;
import com.unity3d.player.o;
import java.util.concurrent.Semaphore;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

final class p {
    /* access modifiers changed from: private */

    /* renamed from: a  reason: collision with root package name */
    public UnityPlayer f169a = null;
    /* access modifiers changed from: private */

    /* renamed from: b  reason: collision with root package name */
    public Context f170b = null;
    private a c;
    /* access modifiers changed from: private */
    public final Semaphore d = new Semaphore(0);
    /* access modifiers changed from: private */
    public final Lock e = new ReentrantLock();
    /* access modifiers changed from: private */
    public o f = null;
    /* access modifiers changed from: private */
    public int g = 2;
    private boolean h = false;
    /* access modifiers changed from: private */
    public boolean i = false;

    public interface a {
        void a();
    }

    p(UnityPlayer unityPlayer) {
        this.f169a = unityPlayer;
    }

    /* access modifiers changed from: private */
    public void d() {
        o oVar = this.f;
        if (oVar != null) {
            this.f169a.removeViewFromPlayer(oVar);
            this.i = false;
            this.f.destroyPlayer();
            this.f = null;
            a aVar = this.c;
            if (aVar != null) {
                aVar.a();
            }
        }
    }

    public final void a() {
        this.e.lock();
        o oVar = this.f;
        if (oVar != null) {
            if (this.g == 0) {
                oVar.CancelOnPrepare();
            } else if (this.i) {
                boolean a2 = oVar.a();
                this.h = a2;
                if (!a2) {
                    this.f.pause();
                }
            }
        }
        this.e.unlock();
    }

    public final boolean a(Context context, String str, int i2, int i3, int i4, boolean z, long j, long j2, a aVar) {
        this.e.lock();
        this.c = aVar;
        this.f170b = context;
        this.d.drainPermits();
        this.g = 2;
        final String str2 = str;
        final int i5 = i2;
        final int i6 = i3;
        final int i7 = i4;
        final boolean z2 = z;
        final long j3 = j;
        final long j4 = j2;
        runOnUiThread(new Runnable() {
            public final void run() {
                if (p.this.f != null) {
                    f.Log(5, "Video already playing");
                    int unused = p.this.g = 2;
                    p.this.d.release();
                    return;
                }
                o unused2 = p.this.f = new o(p.this.f170b, str2, i5, i6, i7, z2, j3, j4, new o.a() {
                    public final void a(int i) {
                        p.this.e.lock();
                        int unused = p.this.g = i;
                        if (i == 3 && p.this.i) {
                            p.this.runOnUiThread(new Runnable() {
                                public final void run() {
                                    p.this.d();
                                    p.this.f169a.resume();
                                }
                            });
                        }
                        if (i != 0) {
                            p.this.d.release();
                        }
                        p.this.e.unlock();
                    }
                });
                if (p.this.f != null) {
                    p.this.f169a.addView(p.this.f);
                }
            }
        });
        boolean z3 = false;
        try {
            this.e.unlock();
            this.d.acquire();
            this.e.lock();
            if (this.g != 2) {
                z3 = true;
            }
        } catch (InterruptedException unused) {
        }
        runOnUiThread(new Runnable() {
            public final void run() {
                p.this.f169a.pause();
            }
        });
        runOnUiThread((!z3 || this.g == 3) ? new Runnable() {
            public final void run() {
                p.this.d();
                p.this.f169a.resume();
            }
        } : new Runnable() {
            public final void run() {
                if (p.this.f != null) {
                    p.this.f169a.addViewToPlayer(p.this.f, true);
                    boolean unused = p.this.i = true;
                    p.this.f.requestFocus();
                }
            }
        });
        this.e.unlock();
        return z3;
    }

    public final void b() {
        this.e.lock();
        o oVar = this.f;
        if (oVar != null && this.i && !this.h) {
            oVar.start();
        }
        this.e.unlock();
    }

    public final void c() {
        this.e.lock();
        o oVar = this.f;
        if (oVar != null) {
            oVar.updateVideoLayout();
        }
        this.e.unlock();
    }

    /* access modifiers changed from: protected */
    public final void runOnUiThread(Runnable runnable) {
        Context context = this.f170b;
        if (context instanceof Activity) {
            ((Activity) context).runOnUiThread(runnable);
        } else {
            f.Log(5, "Not running from an Activity; Ignoring execution request...");
        }
    }
}
