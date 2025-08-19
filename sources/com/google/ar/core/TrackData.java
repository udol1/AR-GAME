package com.google.ar.core;

import java.nio.ByteBuffer;

public class TrackData {
    long nativeHandle;
    final long nativeSymbolTableHandle;
    private final Session session;

    TrackData(long j, Session session2) {
        this.session = session2;
        this.nativeHandle = j;
        this.nativeSymbolTableHandle = session2.nativeSymbolTableHandle;
    }

    private native ByteBuffer nativeGetData(long j, long j2);

    private native long nativeGetFrameTimestamp(long j, long j2);

    private static native void nativeReleaseTrackData(long j, long j2);

    private void release() {
        long j = this.nativeHandle;
        if (j != 0) {
            nativeReleaseTrackData(this.nativeSymbolTableHandle, j);
            this.nativeHandle = 0;
        }
    }

    public void close() {
        release();
    }

    /* access modifiers changed from: protected */
    public void finalize() throws Throwable {
        release();
        super.finalize();
    }

    public ByteBuffer getData() {
        return nativeGetData(this.session.nativeWrapperHandle, this.nativeHandle);
    }

    public long getFrameTimestamp() {
        return nativeGetFrameTimestamp(this.session.nativeWrapperHandle, this.nativeHandle);
    }
}
