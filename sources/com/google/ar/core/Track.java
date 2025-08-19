package com.google.ar.core;

import java.nio.ByteBuffer;
import java.util.UUID;

public class Track {
    long nativeHandle;
    private final long nativeSymbolTableHandle;
    private final Session session;

    protected Track() {
        this.session = null;
        this.nativeHandle = 0;
        this.nativeSymbolTableHandle = 0;
    }

    public Track(Session session2) {
        this.session = session2;
        this.nativeHandle = nativeCreateTrack(session2.nativeWrapperHandle);
        this.nativeSymbolTableHandle = session2.nativeSymbolTableHandle;
    }

    private static native long nativeCreateTrack(long j);

    private static native void nativeDestroyTrack(long j, long j2);

    private native void nativeSetId(long j, long j2, byte[] bArr);

    private native void nativeSetMetadata(long j, long j2, byte[] bArr);

    private native void nativeSetMimeType(long j, long j2, String str);

    /* access modifiers changed from: protected */
    public void finalize() throws Throwable {
        long j = this.nativeHandle;
        if (j != 0) {
            nativeDestroyTrack(this.nativeSymbolTableHandle, j);
            this.nativeHandle = 0;
        }
        super.finalize();
    }

    public Track setId(UUID uuid) {
        ByteBuffer wrap = ByteBuffer.wrap(new byte[16]);
        wrap.putLong(uuid.getMostSignificantBits());
        wrap.putLong(uuid.getLeastSignificantBits());
        nativeSetId(this.session.nativeWrapperHandle, this.nativeHandle, wrap.array());
        return this;
    }

    public Track setMetadata(ByteBuffer byteBuffer) {
        byteBuffer.position(0);
        byte[] bArr = new byte[byteBuffer.remaining()];
        byteBuffer.get(bArr);
        nativeSetMetadata(this.session.nativeWrapperHandle, this.nativeHandle, bArr);
        return this;
    }

    public Track setMimeType(String str) {
        nativeSetMimeType(this.session.nativeWrapperHandle, this.nativeHandle, str);
        return this;
    }
}
