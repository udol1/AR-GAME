package com.google.ar.core;

import a.a.a;
import com.google.ar.core.exceptions.FatalException;
import java.nio.ByteBuffer;

/* compiled from: ArImage */
final class l extends a {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ ArImage f26a;

    /* renamed from: b  reason: collision with root package name */
    private final long f27b;
    private final int c;

    public l(ArImage arImage, long j, int i) {
        this.f26a = arImage;
        this.f27b = j;
        this.c = i;
    }

    public final ByteBuffer getBuffer() {
        ArImage arImage = this.f26a;
        return arImage.nativeGetBuffer(arImage.session.nativeWrapperHandle, this.f27b, this.c).asReadOnlyBuffer();
    }

    public final int getPixelStride() {
        ArImage arImage = this.f26a;
        int access$200 = arImage.nativeGetPixelStride(arImage.session.nativeWrapperHandle, this.f27b, this.c);
        if (access$200 != -1) {
            return access$200;
        }
        throw new FatalException("Unknown error in ArImage.Plane.getPixelStride().");
    }

    public final int getRowStride() {
        ArImage arImage = this.f26a;
        int access$100 = arImage.nativeGetRowStride(arImage.session.nativeWrapperHandle, this.f27b, this.c);
        if (access$100 != -1) {
            return access$100;
        }
        throw new FatalException("Unknown error in ArImage.Plane.getRowStride().");
    }
}
