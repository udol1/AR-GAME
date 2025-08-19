package com.google.ar.core;

public class CameraIntrinsics {
    long nativeHandle;
    private final long nativeSymbolTableHandle;
    private final Session session;

    protected CameraIntrinsics() {
        this.nativeHandle = 0;
        this.session = null;
        this.nativeSymbolTableHandle = 0;
    }

    CameraIntrinsics(long j, Session session2) {
        this.nativeHandle = j;
        this.session = session2;
        this.nativeSymbolTableHandle = session2.nativeSymbolTableHandle;
    }

    private native long nativeCreateIntrinsics(long j, float f, float f2, float f3, float f4, int i, int i2);

    private native void nativeDestroyCameraIntrinsics(long j, long j2);

    private native void nativeGetFocalLength(long j, long j2, float[] fArr, int i);

    private native void nativeGetImageDimensions(long j, long j2, int[] iArr, int i);

    private native void nativeGetPrincipalPoint(long j, long j2, float[] fArr, int i);

    /* access modifiers changed from: protected */
    public void finalize() throws Throwable {
        long j = this.nativeHandle;
        if (j != 0) {
            nativeDestroyCameraIntrinsics(this.nativeSymbolTableHandle, j);
        }
        super.finalize();
    }

    public float[] getFocalLength() {
        float[] fArr = new float[2];
        getFocalLength(fArr, 0);
        return fArr;
    }

    public int[] getImageDimensions() {
        int[] iArr = new int[2];
        getImageDimensions(iArr, 0);
        return iArr;
    }

    public float[] getPrincipalPoint() {
        float[] fArr = new float[2];
        getPrincipalPoint(fArr, 0);
        return fArr;
    }

    public void getFocalLength(float[] fArr, int i) {
        nativeGetFocalLength(this.session.nativeWrapperHandle, this.nativeHandle, fArr, i);
    }

    public void getImageDimensions(int[] iArr, int i) {
        nativeGetImageDimensions(this.session.nativeWrapperHandle, this.nativeHandle, iArr, i);
    }

    public void getPrincipalPoint(float[] fArr, int i) {
        nativeGetPrincipalPoint(this.session.nativeWrapperHandle, this.nativeHandle, fArr, i);
    }
}
