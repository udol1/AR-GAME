package com.unity3d.player;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioManager;

public class HFPStatus {

    /* renamed from: a  reason: collision with root package name */
    private Context f66a;

    /* renamed from: b  reason: collision with root package name */
    private BroadcastReceiver f67b = null;
    private Intent c = null;
    /* access modifiers changed from: private */
    public boolean d = false;
    /* access modifiers changed from: private */
    public AudioManager e = null;
    /* access modifiers changed from: private */
    public int f = a.f69a;

    enum a {
        ;

        static {
            d = new int[]{1, 2, 3};
        }
    }

    public HFPStatus(Context context) {
        this.f66a = context;
        this.e = (AudioManager) context.getSystemService("audio");
        initHFPStatusJni();
    }

    private final native void deinitHFPStatusJni();

    private final native void initHFPStatusJni();

    public final void a() {
        deinitHFPStatusJni();
    }

    /* access modifiers changed from: protected */
    public boolean getHFPStat() {
        return this.f == a.f70b;
    }

    /* access modifiers changed from: protected */
    public void requestHFPStat() {
        AnonymousClass1 r0 = new BroadcastReceiver() {
            public void onReceive(Context context, Intent intent) {
                int intExtra = intent.getIntExtra("android.media.extra.SCO_AUDIO_STATE", -1);
                if (intExtra == 0) {
                    if (HFPStatus.this.d) {
                        HFPStatus.this.e.setMode(0);
                    }
                    boolean unused = HFPStatus.this.d = false;
                } else if (intExtra == 1) {
                    int unused2 = HFPStatus.this.f = a.f70b;
                    if (!HFPStatus.this.d) {
                        HFPStatus.this.e.stopBluetoothSco();
                    } else {
                        HFPStatus.this.e.setMode(3);
                    }
                } else if (intExtra == 2) {
                    if (HFPStatus.this.f == a.f70b) {
                        boolean unused3 = HFPStatus.this.d = true;
                    } else {
                        int unused4 = HFPStatus.this.f = a.c;
                    }
                }
            }
        };
        this.f67b = r0;
        this.c = this.f66a.registerReceiver(r0, new IntentFilter("android.media.ACTION_SCO_AUDIO_STATE_UPDATED"));
        try {
            this.e.startBluetoothSco();
        } catch (NullPointerException unused) {
            f.Log(5, "startBluetoothSco() failed. no bluetooth device connected.");
        }
    }
}
