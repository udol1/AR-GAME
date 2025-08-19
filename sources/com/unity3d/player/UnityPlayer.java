package com.unity3d.player;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.BroadcastReceiver;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.res.Configuration;
import android.graphics.Rect;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.os.Process;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyManager;
import android.util.TypedValue;
import android.view.InputEvent;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.widget.FrameLayout;
import com.ttt.ArCoreTest.BuildConfig;
import com.unity3d.player.k;
import com.unity3d.player.p;
import java.io.UnsupportedEncodingException;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;

public class UnityPlayer extends FrameLayout implements e {
    public static Activity currentActivity;
    private static boolean v;

    /* renamed from: a  reason: collision with root package name */
    e f80a = new e(this, (byte) 0);

    /* renamed from: b  reason: collision with root package name */
    j f81b = null;
    /* access modifiers changed from: private */
    public int c = -1;
    /* access modifiers changed from: private */
    public boolean d = false;
    private boolean e = true;
    private m f = new m();
    private final ConcurrentLinkedQueue g = new ConcurrentLinkedQueue();
    private BroadcastReceiver h = null;
    private boolean i = false;
    private c j = new c(this, (byte) 0);
    private TelephonyManager k;
    private ClipboardManager l;
    /* access modifiers changed from: private */
    public k m;
    private GoogleARCoreApi n = null;
    private a o = new a();
    private Camera2Wrapper p = null;
    private HFPStatus q = null;
    private Uri r = null;
    private NetworkConnectivity s = null;
    /* access modifiers changed from: private */
    public Context t;
    /* access modifiers changed from: private */
    public SurfaceView u;
    /* access modifiers changed from: private */
    public boolean w;
    private boolean x = true;
    /* access modifiers changed from: private */
    public p y;

    class a implements SensorEventListener {
        a() {
        }

        public final void onAccuracyChanged(Sensor sensor, int i) {
        }

        public final void onSensorChanged(SensorEvent sensorEvent) {
        }
    }

    enum b {
        ;

        static {
            d = new int[]{1, 2, 3};
        }
    }

    private class c extends PhoneStateListener {
        private c() {
        }

        /* synthetic */ c(UnityPlayer unityPlayer, byte b2) {
            this();
        }

        public final void onCallStateChanged(int i, String str) {
            UnityPlayer unityPlayer = UnityPlayer.this;
            boolean z = true;
            if (i != 1) {
                z = false;
            }
            unityPlayer.nativeMuteMasterAudio(z);
        }
    }

    enum d {
        PAUSE,
        RESUME,
        QUIT,
        SURFACE_LOST,
        SURFACE_ACQUIRED,
        FOCUS_LOST,
        FOCUS_GAINED,
        NEXT_FRAME,
        URL_ACTIVATED
    }

    private class e extends Thread {

        /* renamed from: a  reason: collision with root package name */
        Handler f127a;

        /* renamed from: b  reason: collision with root package name */
        boolean f128b;
        boolean c;
        int d;
        int e;

        private e() {
            this.f128b = false;
            this.c = false;
            this.d = b.f123b;
            this.e = 5;
        }

        /* synthetic */ e(UnityPlayer unityPlayer, byte b2) {
            this();
        }

        private void a(d dVar) {
            Handler handler = this.f127a;
            if (handler != null) {
                Message.obtain(handler, 2269, dVar).sendToTarget();
            }
        }

        public final void a() {
            a(d.QUIT);
        }

        public final void a(Runnable runnable) {
            if (this.f127a != null) {
                a(d.PAUSE);
                Message.obtain(this.f127a, runnable).sendToTarget();
            }
        }

        public final void b() {
            a(d.RESUME);
        }

        public final void b(Runnable runnable) {
            if (this.f127a != null) {
                a(d.SURFACE_LOST);
                Message.obtain(this.f127a, runnable).sendToTarget();
            }
        }

        public final void c() {
            a(d.FOCUS_GAINED);
        }

        public final void c(Runnable runnable) {
            Handler handler = this.f127a;
            if (handler != null) {
                Message.obtain(handler, runnable).sendToTarget();
                a(d.SURFACE_ACQUIRED);
            }
        }

        public final void d() {
            a(d.FOCUS_LOST);
        }

        public final void d(Runnable runnable) {
            Handler handler = this.f127a;
            if (handler != null) {
                Message.obtain(handler, runnable).sendToTarget();
            }
        }

        public final void e() {
            a(d.URL_ACTIVATED);
        }

        public final void run() {
            setName("UnityMain");
            Looper.prepare();
            this.f127a = new Handler(new Handler.Callback() {
                private void a() {
                    if (e.this.d == b.c && e.this.c) {
                        UnityPlayer.this.nativeFocusChanged(true);
                        e.this.d = b.f122a;
                    }
                }

                public final boolean handleMessage(Message message) {
                    if (message.what != 2269) {
                        return false;
                    }
                    d dVar = (d) message.obj;
                    if (dVar == d.NEXT_FRAME) {
                        UnityPlayer.this.executeGLThreadJobs();
                        if (!e.this.f128b || !e.this.c) {
                            return true;
                        }
                        if (e.this.e >= 0) {
                            if (e.this.e == 0 && UnityPlayer.this.k()) {
                                UnityPlayer.this.a();
                            }
                            e.this.e--;
                        }
                        if (!UnityPlayer.this.isFinishing() && !UnityPlayer.this.nativeRender()) {
                            UnityPlayer.this.e();
                        }
                    } else if (dVar == d.QUIT) {
                        Looper.myLooper().quit();
                    } else if (dVar == d.RESUME) {
                        e.this.f128b = true;
                    } else if (dVar == d.PAUSE) {
                        e.this.f128b = false;
                    } else if (dVar == d.SURFACE_LOST) {
                        e.this.c = false;
                    } else {
                        if (dVar == d.SURFACE_ACQUIRED) {
                            e.this.c = true;
                        } else if (dVar == d.FOCUS_LOST) {
                            if (e.this.d == b.f122a) {
                                UnityPlayer.this.nativeFocusChanged(false);
                            }
                            e.this.d = b.f123b;
                        } else if (dVar == d.FOCUS_GAINED) {
                            e.this.d = b.c;
                        } else if (dVar == d.URL_ACTIVATED) {
                            UnityPlayer.this.nativeSetLaunchURL(UnityPlayer.this.getLaunchURL());
                        }
                        a();
                    }
                    if (e.this.f128b) {
                        Message.obtain(e.this.f127a, 2269, d.NEXT_FRAME).sendToTarget();
                    }
                    return true;
                }
            });
            Looper.loop();
        }
    }

    private abstract class f implements Runnable {
        private f() {
        }

        /* synthetic */ f(UnityPlayer unityPlayer, byte b2) {
            this();
        }

        public abstract void a();

        public final void run() {
            if (!UnityPlayer.this.isFinishing()) {
                a();
            }
        }
    }

    static {
        new l().a();
        v = false;
        v = loadLibraryStatic("main");
    }

    public UnityPlayer(Context context) {
        super(context);
        if (context instanceof Activity) {
            Activity activity = (Activity) context;
            currentActivity = activity;
            this.c = activity.getRequestedOrientation();
            this.r = currentActivity.getIntent().getData();
        }
        a(currentActivity);
        this.t = context;
        if (currentActivity != null && k()) {
            k kVar = new k(this.t, k.a.a()[getSplashMode()]);
            this.m = kVar;
            addView(kVar);
        }
        a(this.t.getApplicationInfo());
        if (!m.c()) {
            AlertDialog create = new AlertDialog.Builder(this.t).setTitle("Failure to initialize!").setPositiveButton("OK", new DialogInterface.OnClickListener() {
                public final void onClick(DialogInterface dialogInterface, int i) {
                    UnityPlayer.this.e();
                }
            }).setMessage("Your hardware does not support this application, sorry!").create();
            create.setCancelable(false);
            create.show();
            return;
        }
        initJni(context);
        this.f.c(true);
        SurfaceView c2 = c();
        this.u = c2;
        c2.setContentDescription(a(context));
        addView(this.u);
        bringChildToFront(this.m);
        this.w = false;
        m();
        this.k = (TelephonyManager) this.t.getSystemService("phone");
        this.l = (ClipboardManager) this.t.getSystemService("clipboard");
        this.p = new Camera2Wrapper(this.t);
        this.q = new HFPStatus(this.t);
        this.f80a.start();
    }

    public static void UnitySendMessage(String str, String str2, String str3) {
        if (!m.c()) {
            f.Log(5, "Native libraries not loaded - dropping message for " + str + "." + str2);
            return;
        }
        try {
            nativeUnitySendMessage(str, str2, str3.getBytes("UTF-8"));
        } catch (UnsupportedEncodingException unused) {
        }
    }

    private static String a(Context context) {
        return context.getResources().getString(context.getResources().getIdentifier("game_view_content_description", "string", context.getPackageName()));
    }

    /* access modifiers changed from: private */
    public void a() {
        a((Runnable) new Runnable() {
            public final void run() {
                UnityPlayer unityPlayer = UnityPlayer.this;
                unityPlayer.removeView(unityPlayer.m);
                k unused = UnityPlayer.this.m = null;
            }
        });
    }

    /* access modifiers changed from: private */
    public void a(int i2, Surface surface) {
        if (!this.d) {
            b(0, surface);
        }
    }

    private static void a(Activity activity) {
        View decorView;
        if (activity != null && activity.getIntent().getBooleanExtra("android.intent.extra.VR_LAUNCH", false) && activity.getWindow() != null && (decorView = activity.getWindow().getDecorView()) != null) {
            decorView.setSystemUiVisibility(7);
        }
    }

    private static void a(ApplicationInfo applicationInfo) {
        if (v && NativeLoader.load(applicationInfo.nativeLibraryDir)) {
            m.a();
        }
    }

    /* JADX WARNING: type inference failed for: r2v0, types: [android.view.ViewParent] */
    /* JADX WARNING: Multi-variable type inference failed */
    /* JADX WARNING: Unknown variable types count: 1 */
    /* Code decompiled incorrectly, please refer to instructions dump. */
    private void a(android.view.View r5, android.view.View r6) {
        /*
            r4 = this;
            com.unity3d.player.m r0 = r4.f
            boolean r0 = r0.d()
            r1 = 0
            if (r0 != 0) goto L_0x000e
            r4.pause()
            r0 = 1
            goto L_0x000f
        L_0x000e:
            r0 = r1
        L_0x000f:
            if (r5 == 0) goto L_0x0030
            android.view.ViewParent r2 = r5.getParent()
            boolean r3 = r2 instanceof com.unity3d.player.UnityPlayer
            if (r3 == 0) goto L_0x001e
            r3 = r2
            com.unity3d.player.UnityPlayer r3 = (com.unity3d.player.UnityPlayer) r3
            if (r3 == r4) goto L_0x0030
        L_0x001e:
            boolean r3 = r2 instanceof android.view.ViewGroup
            if (r3 == 0) goto L_0x0027
            android.view.ViewGroup r2 = (android.view.ViewGroup) r2
            r2.removeView(r5)
        L_0x0027:
            r4.addView(r5)
            r4.bringChildToFront(r5)
            r5.setVisibility(r1)
        L_0x0030:
            if (r6 == 0) goto L_0x0040
            android.view.ViewParent r5 = r6.getParent()
            if (r5 != r4) goto L_0x0040
            r5 = 8
            r6.setVisibility(r5)
            r4.removeView(r6)
        L_0x0040:
            if (r0 == 0) goto L_0x0045
            r4.resume()
        L_0x0045:
            return
        */
        throw new UnsupportedOperationException("Method not decompiled: com.unity3d.player.UnityPlayer.a(android.view.View, android.view.View):void");
    }

    private void a(f fVar) {
        if (!isFinishing()) {
            b((Runnable) fVar);
        }
    }

    private void b(Runnable runnable) {
        if (m.c()) {
            if (Thread.currentThread() == this.f80a) {
                runnable.run();
            } else {
                this.g.add(runnable);
            }
        }
    }

    private static boolean b() {
        if (currentActivity == null) {
            return false;
        }
        TypedValue typedValue = new TypedValue();
        return currentActivity.getTheme().resolveAttribute(16842840, typedValue, true) && typedValue.type == 18 && typedValue.data != 0;
    }

    private boolean b(final int i2, final Surface surface) {
        if (!m.c() || !this.f.e()) {
            return false;
        }
        final Semaphore semaphore = new Semaphore(0);
        AnonymousClass22 r1 = new Runnable() {
            public final void run() {
                UnityPlayer.this.nativeRecreateGfxState(i2, surface);
                semaphore.release();
            }
        };
        if (i2 == 0) {
            e eVar = this.f80a;
            if (surface == null) {
                eVar.b(r1);
            } else {
                eVar.c(r1);
            }
        } else {
            r1.run();
        }
        if (surface != null || i2 != 0) {
            return true;
        }
        try {
            if (semaphore.tryAcquire(4, TimeUnit.SECONDS)) {
                return true;
            }
            f.Log(5, "Timeout while trying detaching primary window.");
            return true;
        } catch (InterruptedException unused) {
            f.Log(5, "UI thread got interrupted while trying to detach the primary window from the Unity Engine.");
            return true;
        }
    }

    /* access modifiers changed from: private */
    public SurfaceView c() {
        SurfaceView surfaceView = new SurfaceView(this.t);
        if (b()) {
            surfaceView.getHolder().setFormat(-3);
            surfaceView.setZOrderOnTop(true);
        } else {
            surfaceView.getHolder().setFormat(-1);
        }
        surfaceView.getHolder().addCallback(new SurfaceHolder.Callback() {
            public final void surfaceChanged(SurfaceHolder surfaceHolder, int i, int i2, int i3) {
                UnityPlayer.this.a(0, surfaceHolder.getSurface());
                UnityPlayer.this.d();
            }

            public final void surfaceCreated(SurfaceHolder surfaceHolder) {
                UnityPlayer.this.a(0, surfaceHolder.getSurface());
            }

            public final void surfaceDestroyed(SurfaceHolder surfaceHolder) {
                UnityPlayer.this.a(0, (Surface) null);
            }
        });
        surfaceView.setFocusable(true);
        surfaceView.setFocusableInTouchMode(true);
        return surfaceView;
    }

    /* access modifiers changed from: private */
    public void d() {
        if (m.c() && this.f.e()) {
            this.f80a.d(new Runnable() {
                public final void run() {
                    UnityPlayer.this.nativeSendSurfaceChangedEvent();
                }
            });
        }
    }

    /* access modifiers changed from: private */
    public void e() {
        Context context = this.t;
        if ((context instanceof Activity) && !((Activity) context).isFinishing()) {
            ((Activity) this.t).finish();
        }
    }

    private void f() {
        reportSoftInputStr((String) null, 1, true);
        if (this.f.g()) {
            if (m.c()) {
                final Semaphore semaphore = new Semaphore(0);
                this.f80a.a(isFinishing() ? new Runnable() {
                    public final void run() {
                        UnityPlayer.this.g();
                        semaphore.release();
                    }
                } : new Runnable() {
                    public final void run() {
                        if (UnityPlayer.this.nativePause()) {
                            boolean unused = UnityPlayer.this.w = true;
                            UnityPlayer.this.g();
                            semaphore.release(2);
                            return;
                        }
                        semaphore.release();
                    }
                });
                try {
                    if (!semaphore.tryAcquire(4, TimeUnit.SECONDS)) {
                        f.Log(5, "Timeout while trying to pause the Unity Engine.");
                    }
                } catch (InterruptedException unused) {
                    f.Log(5, "UI thread got interrupted while trying to pause the Unity Engine.");
                }
                if (semaphore.drainPermits() > 0) {
                    destroy();
                }
            }
            this.f.d(false);
            this.f.b(true);
            if (this.i) {
                this.k.listen(this.j, 0);
            }
        }
    }

    /* access modifiers changed from: private */
    public void g() {
        this.x = nativeDone();
        this.f.c(false);
    }

    private void h() {
        if (this.f.f()) {
            this.f.d(true);
            b((Runnable) new Runnable() {
                public final void run() {
                    UnityPlayer.this.nativeResume();
                }
            });
            this.f80a.b();
        }
    }

    private static void i() {
        if (m.c()) {
            if (NativeLoader.unload()) {
                m.b();
                return;
            }
            throw new UnsatisfiedLinkError("Unable to unload libraries from libmain.so");
        }
    }

    private final native void initJni(Context context);

    private ApplicationInfo j() {
        return this.t.getPackageManager().getApplicationInfo(this.t.getPackageName(), 128);
    }

    /* access modifiers changed from: private */
    public boolean k() {
        try {
            return j().metaData.getBoolean("unity.splash-enable");
        } catch (Exception unused) {
            return false;
        }
    }

    private boolean l() {
        try {
            return j().metaData.getBoolean("unity.tango-enable");
        } catch (Exception unused) {
            return false;
        }
    }

    protected static boolean loadLibraryStatic(String str) {
        StringBuilder sb;
        try {
            System.loadLibrary(str);
            return true;
        } catch (UnsatisfiedLinkError unused) {
            sb = new StringBuilder("Unable to find ");
            sb.append(str);
            f.Log(6, sb.toString());
            return false;
        } catch (Exception e2) {
            sb = new StringBuilder("Unknown error ");
            sb.append(e2);
            f.Log(6, sb.toString());
            return false;
        }
    }

    private void m() {
        Context context = this.t;
        if (context instanceof Activity) {
            ((Activity) context).getWindow().setFlags(1024, 1024);
        }
    }

    private final native boolean nativeDone();

    /* access modifiers changed from: private */
    public final native void nativeFocusChanged(boolean z);

    private final native boolean nativeInjectEvent(InputEvent inputEvent);

    /* access modifiers changed from: private */
    public final native boolean nativeIsAutorotationOn();

    /* access modifiers changed from: private */
    public final native void nativeLowMemory();

    /* access modifiers changed from: private */
    public final native void nativeMuteMasterAudio(boolean z);

    /* access modifiers changed from: private */
    public final native boolean nativePause();

    /* access modifiers changed from: private */
    public final native void nativeRecreateGfxState(int i2, Surface surface);

    /* access modifiers changed from: private */
    public final native boolean nativeRender();

    private final native void nativeRestartActivityIndicator();

    /* access modifiers changed from: private */
    public final native void nativeResume();

    /* access modifiers changed from: private */
    public final native void nativeSendSurfaceChangedEvent();

    /* access modifiers changed from: private */
    public final native void nativeSetInputArea(int i2, int i3, int i4, int i5);

    /* access modifiers changed from: private */
    public final native void nativeSetInputSelection(int i2, int i3);

    /* access modifiers changed from: private */
    public final native void nativeSetInputString(String str);

    /* access modifiers changed from: private */
    public final native void nativeSetKeyboardIsVisible(boolean z);

    /* access modifiers changed from: private */
    public final native void nativeSetLaunchURL(String str);

    /* access modifiers changed from: private */
    public final native void nativeSoftInputCanceled();

    /* access modifiers changed from: private */
    public final native void nativeSoftInputClosed();

    private final native void nativeSoftInputLostFocus();

    private static native void nativeUnitySendMessage(String str, String str2, byte[] bArr);

    /* access modifiers changed from: package-private */
    public final void a(Runnable runnable) {
        Context context = this.t;
        if (context instanceof Activity) {
            ((Activity) context).runOnUiThread(runnable);
        } else {
            f.Log(5, "Not running Unity from an Activity; ignored...");
        }
    }

    /* access modifiers changed from: protected */
    public void addPhoneCallListener() {
        this.i = true;
        this.k.listen(this.j, 32);
    }

    public boolean addViewToPlayer(View view, boolean z) {
        a(view, (View) z ? this.u : null);
        boolean z2 = true;
        boolean z3 = view.getParent() == this;
        boolean z4 = z && this.u.getParent() == null;
        boolean z5 = this.u.getParent() == this;
        if (!z3 || (!z4 && !z5)) {
            z2 = false;
        }
        if (!z2) {
            if (!z3) {
                f.Log(6, "addViewToPlayer: Failure adding view to hierarchy");
            }
            if (!z4 && !z5) {
                f.Log(6, "addViewToPlayer: Failure removing old view from hierarchy");
            }
        }
        return z2;
    }

    public void configurationChanged(Configuration configuration) {
        SurfaceView surfaceView = this.u;
        if (surfaceView instanceof SurfaceView) {
            surfaceView.getHolder().setSizeFromLayout();
        }
        p pVar = this.y;
        if (pVar != null) {
            pVar.c();
        }
        GoogleVrProxy b2 = GoogleVrApi.b();
        if (b2 != null) {
            b2.c();
        }
    }

    public void destroy() {
        if (GoogleVrApi.b() != null) {
            GoogleVrApi.a();
        }
        Camera2Wrapper camera2Wrapper = this.p;
        if (camera2Wrapper != null) {
            camera2Wrapper.a();
            this.p = null;
        }
        HFPStatus hFPStatus = this.q;
        if (hFPStatus != null) {
            hFPStatus.a();
            this.q = null;
        }
        NetworkConnectivity networkConnectivity = this.s;
        if (networkConnectivity != null) {
            networkConnectivity.b();
            this.s = null;
        }
        this.w = true;
        if (!this.f.d()) {
            pause();
        }
        this.f80a.a();
        try {
            this.f80a.join(4000);
        } catch (InterruptedException unused) {
            this.f80a.interrupt();
        }
        BroadcastReceiver broadcastReceiver = this.h;
        if (broadcastReceiver != null) {
            this.t.unregisterReceiver(broadcastReceiver);
        }
        this.h = null;
        if (m.c()) {
            removeAllViews();
        }
        if (this.x) {
            kill();
        }
        i();
    }

    /* access modifiers changed from: protected */
    public void disableLogger() {
        f.f143a = true;
    }

    public boolean displayChanged(int i2, Surface surface) {
        if (i2 == 0) {
            this.d = surface != null;
            a((Runnable) new Runnable() {
                public final void run() {
                    if (UnityPlayer.this.d) {
                        UnityPlayer unityPlayer = UnityPlayer.this;
                        unityPlayer.removeView(unityPlayer.u);
                        return;
                    }
                    UnityPlayer unityPlayer2 = UnityPlayer.this;
                    unityPlayer2.addView(unityPlayer2.u);
                }
            });
        }
        return b(i2, surface);
    }

    /* access modifiers changed from: protected */
    public void executeGLThreadJobs() {
        while (true) {
            Runnable runnable = (Runnable) this.g.poll();
            if (runnable != null) {
                runnable.run();
            } else {
                return;
            }
        }
    }

    /* access modifiers changed from: protected */
    public String getClipboardText() {
        ClipData primaryClip = this.l.getPrimaryClip();
        return primaryClip != null ? primaryClip.getItemAt(0).coerceToText(this.t).toString() : BuildConfig.FLAVOR;
    }

    /* access modifiers changed from: protected */
    public String getLaunchURL() {
        Uri uri = this.r;
        if (uri != null) {
            return uri.toString();
        }
        return null;
    }

    /* access modifiers changed from: protected */
    public int getNetworkConnectivity() {
        if (!i.d) {
            return 0;
        }
        if (this.s == null) {
            this.s = new NetworkConnectivity(this.t);
        }
        return this.s.a();
    }

    public Bundle getSettings() {
        return Bundle.EMPTY;
    }

    /* access modifiers changed from: protected */
    public int getSplashMode() {
        try {
            return j().metaData.getInt("unity.splash-mode");
        } catch (Exception unused) {
            return 0;
        }
    }

    public View getView() {
        return this;
    }

    /* access modifiers changed from: protected */
    public void hideSoftInput() {
        reportSoftInputArea(new Rect());
        reportSoftInputIsVisible(false);
        final AnonymousClass5 r0 = new Runnable() {
            public final void run() {
                if (UnityPlayer.this.f81b != null) {
                    UnityPlayer.this.f81b.dismiss();
                    UnityPlayer.this.f81b = null;
                }
            }
        };
        if (i.f145b) {
            a((f) new f() {
                public final void a() {
                    UnityPlayer.this.a(r0);
                }
            });
        } else {
            a((Runnable) r0);
        }
    }

    public void init(int i2, boolean z) {
    }

    /* access modifiers changed from: protected */
    public boolean initializeGoogleAr() {
        if (this.n != null || currentActivity == null || !l()) {
            return false;
        }
        GoogleARCoreApi googleARCoreApi = new GoogleARCoreApi();
        this.n = googleARCoreApi;
        googleARCoreApi.initializeARCore(currentActivity);
        if (this.f.d()) {
            return false;
        }
        this.n.resumeARCore();
        return false;
    }

    /* access modifiers changed from: protected */
    public boolean initializeGoogleVr() {
        final GoogleVrProxy b2 = GoogleVrApi.b();
        if (b2 == null) {
            GoogleVrApi.a(this);
            b2 = GoogleVrApi.b();
            if (b2 == null) {
                f.Log(6, "Unable to create Google VR subsystem.");
                return false;
            }
        }
        final Semaphore semaphore = new Semaphore(0);
        final AnonymousClass15 r3 = new Runnable() {
            public final void run() {
                UnityPlayer.this.injectEvent(new KeyEvent(0, 4));
                UnityPlayer.this.injectEvent(new KeyEvent(1, 4));
            }
        };
        a((Runnable) new Runnable() {
            public final void run() {
                if (!b2.a(UnityPlayer.currentActivity, UnityPlayer.this.t, UnityPlayer.this.c(), r3)) {
                    f.Log(6, "Unable to initialize Google VR subsystem.");
                }
                if (UnityPlayer.currentActivity != null) {
                    b2.a(UnityPlayer.currentActivity.getIntent());
                }
                semaphore.release();
            }
        });
        try {
            if (semaphore.tryAcquire(4, TimeUnit.SECONDS)) {
                return b2.a();
            }
            f.Log(5, "Timeout while trying to initialize Google VR.");
            return false;
        } catch (InterruptedException e2) {
            f.Log(5, "UI thread was interrupted while initializing Google VR. " + e2.getLocalizedMessage());
            return false;
        }
    }

    public boolean injectEvent(InputEvent inputEvent) {
        if (!m.c()) {
            return false;
        }
        return nativeInjectEvent(inputEvent);
    }

    /* access modifiers changed from: protected */
    public boolean isFinishing() {
        if (!this.w) {
            Context context = this.t;
            boolean z = (context instanceof Activity) && ((Activity) context).isFinishing();
            this.w = z;
            return z;
        }
    }

    /* access modifiers changed from: protected */
    public void kill() {
        Process.killProcess(Process.myPid());
    }

    /* access modifiers changed from: protected */
    public boolean loadLibrary(String str) {
        return loadLibraryStatic(str);
    }

    public void lowMemory() {
        if (m.c()) {
            b((Runnable) new Runnable() {
                public final void run() {
                    UnityPlayer.this.nativeLowMemory();
                }
            });
        }
    }

    public void newIntent(Intent intent) {
        this.r = intent.getData();
        this.f80a.e();
    }

    public boolean onGenericMotionEvent(MotionEvent motionEvent) {
        return injectEvent(motionEvent);
    }

    public boolean onKeyDown(int i2, KeyEvent keyEvent) {
        return injectEvent(keyEvent);
    }

    public boolean onKeyLongPress(int i2, KeyEvent keyEvent) {
        return injectEvent(keyEvent);
    }

    public boolean onKeyMultiple(int i2, int i3, KeyEvent keyEvent) {
        return injectEvent(keyEvent);
    }

    public boolean onKeyUp(int i2, KeyEvent keyEvent) {
        return injectEvent(keyEvent);
    }

    public boolean onTouchEvent(MotionEvent motionEvent) {
        return injectEvent(motionEvent);
    }

    public void pause() {
        GoogleARCoreApi googleARCoreApi = this.n;
        if (googleARCoreApi != null) {
            googleARCoreApi.pauseARCore();
        }
        p pVar = this.y;
        if (pVar != null) {
            pVar.a();
        }
        GoogleVrProxy b2 = GoogleVrApi.b();
        if (b2 != null) {
            b2.pauseGvrLayout();
        }
        f();
    }

    public void quit() {
        destroy();
    }

    public void removeViewFromPlayer(View view) {
        a((View) this.u, view);
        boolean z = true;
        boolean z2 = view.getParent() == null;
        boolean z3 = this.u.getParent() == this;
        if (!z2 || !z3) {
            z = false;
        }
        if (!z) {
            if (!z2) {
                f.Log(6, "removeViewFromPlayer: Failure removing view from hierarchy");
            }
            if (!z3) {
                f.Log(6, "removeVireFromPlayer: Failure agging old view to hierarchy");
            }
        }
    }

    public void reportError(String str, String str2) {
        f.Log(6, str + ": " + str2);
    }

    /* access modifiers changed from: protected */
    public void reportSoftInputArea(final Rect rect) {
        a((f) new f() {
            public final void a() {
                UnityPlayer.this.nativeSetInputArea(rect.left, rect.top, rect.right, rect.bottom);
            }
        });
    }

    /* access modifiers changed from: protected */
    public void reportSoftInputIsVisible(final boolean z) {
        a((f) new f() {
            public final void a() {
                UnityPlayer.this.nativeSetKeyboardIsVisible(z);
            }
        });
    }

    /* access modifiers changed from: protected */
    public void reportSoftInputSelection(final int i2, final int i3) {
        a((f) new f() {
            public final void a() {
                UnityPlayer.this.nativeSetInputSelection(i2, i3);
            }
        });
    }

    /* access modifiers changed from: protected */
    public void reportSoftInputStr(final String str, final int i2, final boolean z) {
        if (i2 == 1) {
            hideSoftInput();
        }
        a((f) new f() {
            public final void a() {
                if (z) {
                    UnityPlayer.this.nativeSoftInputCanceled();
                } else {
                    String str = str;
                    if (str != null) {
                        UnityPlayer.this.nativeSetInputString(str);
                    }
                }
                if (i2 == 1) {
                    UnityPlayer.this.nativeSoftInputClosed();
                }
            }
        });
    }

    /* access modifiers changed from: protected */
    public void requestUserAuthorization(String str) {
        if (i.c && str != null && !str.isEmpty() && currentActivity != null) {
            i.e.a(currentActivity, str);
        }
    }

    public void resume() {
        GoogleARCoreApi googleARCoreApi = this.n;
        if (googleARCoreApi != null) {
            googleARCoreApi.resumeARCore();
        }
        this.f.b(false);
        p pVar = this.y;
        if (pVar != null) {
            pVar.b();
        }
        h();
        nativeRestartActivityIndicator();
        GoogleVrProxy b2 = GoogleVrApi.b();
        if (b2 != null) {
            b2.b();
        }
    }

    /* access modifiers changed from: protected */
    public void setCharacterLimit(final int i2) {
        a((Runnable) new Runnable() {
            public final void run() {
                if (UnityPlayer.this.f81b != null) {
                    UnityPlayer.this.f81b.a(i2);
                }
            }
        });
    }

    /* access modifiers changed from: protected */
    public void setClipboardText(String str) {
        this.l.setPrimaryClip(ClipData.newPlainText("Text", str));
    }

    /* access modifiers changed from: protected */
    public void setHideInputField(final boolean z) {
        a((Runnable) new Runnable() {
            public final void run() {
                if (UnityPlayer.this.f81b != null) {
                    UnityPlayer.this.f81b.a(z);
                }
            }
        });
    }

    /* access modifiers changed from: protected */
    public void setSelection(final int i2, final int i3) {
        a((Runnable) new Runnable() {
            public final void run() {
                if (UnityPlayer.this.f81b != null) {
                    UnityPlayer.this.f81b.a(i2, i3);
                }
            }
        });
    }

    /* access modifiers changed from: protected */
    public void setSoftInputStr(final String str) {
        a((Runnable) new Runnable() {
            public final void run() {
                if (UnityPlayer.this.f81b != null && str != null) {
                    UnityPlayer.this.f81b.a(str);
                }
            }
        });
    }

    /* access modifiers changed from: protected */
    public void showSoftInput(String str, int i2, boolean z, boolean z2, boolean z3, boolean z4, String str2, int i3, boolean z5) {
        final String str3 = str;
        final int i4 = i2;
        final boolean z6 = z;
        final boolean z7 = z2;
        final boolean z8 = z3;
        final boolean z9 = z4;
        final String str4 = str2;
        final int i5 = i3;
        final boolean z10 = z5;
        a((Runnable) new Runnable() {
            public final void run() {
                UnityPlayer.this.f81b = new j(UnityPlayer.this.t, this, str3, i4, z6, z7, z8, str4, i5, z10);
                UnityPlayer.this.f81b.show();
            }
        });
    }

    /* access modifiers changed from: protected */
    public boolean showVideoPlayer(String str, int i2, int i3, int i4, boolean z, int i5, int i6) {
        if (this.y == null) {
            this.y = new p(this);
        }
        boolean a2 = this.y.a(this.t, str, i2, i3, i4, z, (long) i5, (long) i6, new p.a() {
            public final void a() {
                p unused = UnityPlayer.this.y = null;
            }
        });
        if (a2) {
            a((Runnable) new Runnable() {
                public final void run() {
                    if (UnityPlayer.this.nativeIsAutorotationOn() && (UnityPlayer.this.t instanceof Activity)) {
                        ((Activity) UnityPlayer.this.t).setRequestedOrientation(UnityPlayer.this.c);
                    }
                }
            });
        }
        return a2;
    }

    /* access modifiers changed from: protected */
    public boolean skipPermissionsDialog() {
        if (!i.c || currentActivity == null) {
            return false;
        }
        return i.e.a(currentActivity);
    }

    public void start() {
    }

    public void stop() {
    }

    /* access modifiers changed from: protected */
    public void toggleGyroscopeSensor(boolean z) {
        SensorManager sensorManager = (SensorManager) this.t.getSystemService("sensor");
        Sensor defaultSensor = sensorManager.getDefaultSensor(11);
        if (z) {
            sensorManager.registerListener(this.o, defaultSensor, 1);
        } else {
            sensorManager.unregisterListener(this.o);
        }
    }

    public void windowFocusChanged(boolean z) {
        this.f.a(z);
        if (this.f.e()) {
            if (z && this.f81b != null) {
                nativeSoftInputLostFocus();
                reportSoftInputStr((String) null, 1, false);
            }
            if (z) {
                this.f80a.c();
            } else {
                this.f80a.d();
            }
            h();
        }
    }
}
