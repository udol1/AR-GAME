package org.fmod;

import android.media.AudioRecord;
import android.util.Log;
import java.nio.ByteBuffer;

final class a implements Runnable {

    /* renamed from: a  reason: collision with root package name */
    private final FMODAudioDevice f180a;

    /* renamed from: b  reason: collision with root package name */
    private final ByteBuffer f181b;
    private final int c;
    private final int d;
    private final int e = 2;
    private volatile Thread f;
    private volatile boolean g;
    private AudioRecord h;
    private boolean i;

    a(FMODAudioDevice fMODAudioDevice, int i2, int i3) {
        this.f180a = fMODAudioDevice;
        this.c = i2;
        this.d = i3;
        this.f181b = ByteBuffer.allocateDirect(AudioRecord.getMinBufferSize(i2, i3, 2));
    }

    private void d() {
        AudioRecord audioRecord = this.h;
        if (audioRecord != null) {
            if (audioRecord.getState() == 1) {
                this.h.stop();
            }
            this.h.release();
            this.h = null;
        }
        this.f181b.position(0);
        this.i = false;
    }

    public final int a() {
        return this.f181b.capacity();
    }

    public final void b() {
        if (this.f != null) {
            c();
        }
        this.g = true;
        this.f = new Thread(this);
        this.f.start();
    }

    public final void c() {
        while (this.f != null) {
            this.g = false;
            try {
                this.f.join();
                this.f = null;
            } catch (InterruptedException unused) {
            }
        }
    }

    public final void run() {
        int i2 = 3;
        while (this.g) {
            if (!this.i && i2 > 0) {
                d();
                AudioRecord audioRecord = new AudioRecord(1, this.c, this.d, this.e, this.f181b.capacity());
                this.h = audioRecord;
                int state = audioRecord.getState();
                boolean z = true;
                if (state != 1) {
                    z = false;
                }
                this.i = z;
                if (z) {
                    this.f181b.position(0);
                    this.h.startRecording();
                    i2 = 3;
                } else {
                    Log.e("FMOD", "AudioRecord failed to initialize (status " + this.h.getState() + ")");
                    i2 += -1;
                    d();
                }
            }
            if (this.i && this.h.getRecordingState() == 3) {
                AudioRecord audioRecord2 = this.h;
                ByteBuffer byteBuffer = this.f181b;
                this.f180a.fmodProcessMicData(this.f181b, audioRecord2.read(byteBuffer, byteBuffer.capacity()));
                this.f181b.position(0);
            }
        }
        d();
    }
}
