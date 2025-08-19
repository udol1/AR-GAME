package com.google.ar.core;

import a.a.b;
import android.graphics.Rect;
import android.media.Image;
import com.google.ar.core.exceptions.FatalException;
import java.nio.ByteBuffer;

public class ArImage extends b {
    long nativeHandle;
    private final long nativeSymbolTableHandle;
    /* access modifiers changed from: private */
    public final Session session;

    ArImage(Session session2, long j) {
        this.session = session2;
        this.nativeHandle = j;
        this.nativeSymbolTableHandle = session2.nativeSymbolTableHandle;
    }

    private native void nativeClose(long j, long j2);

    /* access modifiers changed from: private */
    public native ByteBuffer nativeGetBuffer(long j, long j2, int i);

    private native int nativeGetFormat(long j, long j2);

    private native int nativeGetHeight(long j, long j2);

    private native int nativeGetNumberOfPlanes(long j, long j2);

    /* access modifiers changed from: private */
    public native int nativeGetPixelStride(long j, long j2, int i);

    /* access modifiers changed from: private */
    public native int nativeGetRowStride(long j, long j2, int i);

    private native long nativeGetTimestamp(long j, long j2);

    private native int nativeGetWidth(long j, long j2);

    static native void nativeLoadSymbols();

    public void close() {
        nativeClose(this.nativeSymbolTableHandle, this.nativeHandle);
        this.nativeHandle = 0;
    }

    public Rect getCropRect() {
        throw new UnsupportedOperationException("Crop rect is unknown in this image.");
    }

    public int getFormat() {
        int nativeGetFormat = nativeGetFormat(this.session.nativeWrapperHandle, this.nativeHandle);
        if (nativeGetFormat != -1) {
            return nativeGetFormat;
        }
        throw new FatalException("Unknown error in ArImage.getFormat().");
    }

    public int getHeight() {
        int nativeGetHeight = nativeGetHeight(this.session.nativeWrapperHandle, this.nativeHandle);
        if (nativeGetHeight != -1) {
            return nativeGetHeight;
        }
        throw new FatalException("Unknown error in ArImage.getHeight().");
    }

    public Image.Plane[] getPlanes() {
        int nativeGetNumberOfPlanes = nativeGetNumberOfPlanes(this.session.nativeWrapperHandle, this.nativeHandle);
        if (nativeGetNumberOfPlanes != -1) {
            l[] lVarArr = new l[nativeGetNumberOfPlanes];
            for (int i = 0; i < nativeGetNumberOfPlanes; i++) {
                lVarArr[i] = new l(this, this.nativeHandle, i);
            }
            return lVarArr;
        }
        throw new FatalException("Unknown error in ArImage.getPlanes().");
    }

    public long getTimestamp() {
        long nativeGetTimestamp = nativeGetTimestamp(this.session.nativeWrapperHandle, this.nativeHandle);
        if (nativeGetTimestamp != -1) {
            return nativeGetTimestamp;
        }
        throw new FatalException("Unknown error in ArImage.getTimestamp().");
    }

    public int getWidth() {
        int nativeGetWidth = nativeGetWidth(this.session.nativeWrapperHandle, this.nativeHandle);
        if (nativeGetWidth != -1) {
            return nativeGetWidth;
        }
        throw new FatalException("Unknown error in ArImage.getWidth().");
    }

    public void setCropRect(Rect rect) {
        throw new UnsupportedOperationException("This is a read-only image.");
    }

    public void setTimestamp(long j) {
        throw new UnsupportedOperationException("This is a read-only image.");
    }
}
