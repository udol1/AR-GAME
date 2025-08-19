package com.google.ar.core;

class Quaternion {

    /* renamed from: a  reason: collision with root package name */
    public static final Quaternion f5a = new Quaternion();
    private float w = 1.0f;
    private float x = 0.0f;
    private float y = 0.0f;
    private float z = 0.0f;

    public Quaternion() {
        j(0.0f, 0.0f, 0.0f, 1.0f);
    }

    public static Quaternion g(Quaternion quaternion, Quaternion quaternion2, float f) {
        float f2;
        Quaternion quaternion3 = new Quaternion();
        float f3 = (quaternion.x * quaternion2.x) + (quaternion.y * quaternion2.y) + (quaternion.z * quaternion2.z) + (quaternion.w * quaternion2.w);
        if (f3 < 0.0f) {
            Quaternion quaternion4 = new Quaternion(quaternion2);
            f3 = -f3;
            quaternion4.x = -quaternion4.x;
            quaternion4.y = -quaternion4.y;
            quaternion4.z = -quaternion4.z;
            quaternion4.w = -quaternion4.w;
            quaternion2 = quaternion4;
        }
        float acos = (float) Math.acos((double) f3);
        float sqrt = (float) Math.sqrt((double) (1.0f - (f3 * f3)));
        if (((double) Math.abs(sqrt)) > 0.001d) {
            float f4 = 1.0f / sqrt;
            f2 = ((float) Math.sin((double) ((1.0f - f) * acos))) * f4;
            f = ((float) Math.sin((double) (f * acos))) * f4;
        } else {
            f2 = 1.0f - f;
        }
        float f5 = (quaternion.x * f2) + (quaternion2.x * f);
        quaternion3.x = f5;
        float f6 = (quaternion.y * f2) + (quaternion2.y * f);
        quaternion3.y = f6;
        float f7 = (quaternion.z * f2) + (quaternion2.z * f);
        quaternion3.z = f7;
        float f8 = (f2 * quaternion.w) + (f * quaternion2.w);
        quaternion3.w = f8;
        float sqrt2 = (float) (1.0d / Math.sqrt((double) ((((f5 * f5) + (f6 * f6)) + (f7 * f7)) + (f8 * f8))));
        quaternion3.x *= sqrt2;
        quaternion3.y *= sqrt2;
        quaternion3.z *= sqrt2;
        quaternion3.w *= sqrt2;
        return quaternion3;
    }

    public static void i(Quaternion quaternion, float[] fArr, int i, float[] fArr2, int i2) {
        float f = fArr[i];
        float f2 = fArr[i + 1];
        float f3 = fArr[i + 2];
        float f4 = quaternion.x;
        float f5 = quaternion.y;
        float f6 = quaternion.z;
        float f7 = quaternion.w;
        float f8 = ((f7 * f) + (f5 * f3)) - (f6 * f2);
        float f9 = ((f7 * f2) + (f6 * f)) - (f4 * f3);
        float f10 = ((f7 * f3) + (f4 * f2)) - (f5 * f);
        float f11 = -f4;
        float f12 = ((f * f11) - (f2 * f5)) - (f3 * f6);
        float f13 = -f6;
        float f14 = -f5;
        fArr2[i2] = (((f8 * f7) + (f12 * f11)) + (f9 * f13)) - (f10 * f14);
        fArr2[i2 + 1] = (((f9 * f7) + (f12 * f14)) + (f10 * f11)) - (f8 * f13);
        fArr2[i2 + 2] = (((f10 * f7) + (f12 * f13)) + (f8 * f14)) - (f9 * f11);
    }

    public final float a() {
        return this.w;
    }

    public final float b() {
        return this.x;
    }

    public final float c() {
        return this.y;
    }

    public final float d() {
        return this.z;
    }

    public final Quaternion e(Quaternion quaternion) {
        Quaternion quaternion2 = new Quaternion();
        float f = this.x;
        float f2 = quaternion.w;
        float f3 = this.y;
        float f4 = quaternion.z;
        float f5 = this.z;
        float f6 = quaternion.y;
        float f7 = this.w;
        quaternion2.x = (((f * f2) + (f3 * f4)) - (f5 * f6)) + (quaternion.x * f7);
        float f8 = this.x;
        float f9 = -f8;
        float f10 = quaternion.x;
        quaternion2.y = (f9 * f4) + (f3 * f2) + (f5 * f10) + (f6 * f7);
        float f11 = quaternion.y;
        float f12 = this.y;
        quaternion2.z = ((f8 * f11) - (f12 * f10)) + (f5 * f2) + (f4 * f7);
        quaternion2.w = (((f9 * f10) - (f12 * f11)) - (this.z * quaternion.z)) + (f7 * f2);
        return quaternion2;
    }

    public final Quaternion f() {
        return new Quaternion(-this.x, -this.y, -this.z, this.w);
    }

    public final void h(float[] fArr, int i) {
        fArr[i] = this.x;
        fArr[i + 1] = this.y;
        fArr[i + 2] = this.z;
        fArr[i + 3] = this.w;
    }

    public final void j(float f, float f2, float f3, float f4) {
        this.x = f;
        this.y = f2;
        this.z = f3;
        this.w = f4;
    }

    public final void k(float[] fArr, int i) {
        float f = this.x;
        float f2 = this.y;
        float f3 = this.z;
        float f4 = this.w;
        float f5 = (f * f) + (f2 * f2) + (f3 * f3) + (f4 * f4);
        float f6 = 0.0f;
        if (f5 > 0.0f) {
            f6 = 2.0f / f5;
        }
        float f7 = f * f6;
        float f8 = f2 * f6;
        float f9 = f6 * f3;
        float f10 = f4 * f7;
        float f11 = f4 * f8;
        float f12 = f4 * f9;
        float f13 = f7 * f;
        float f14 = f * f8;
        float f15 = f * f9;
        float f16 = f8 * f2;
        float f17 = f2 * f9;
        float f18 = f3 * f9;
        fArr[i] = 1.0f - (f16 + f18);
        fArr[i + 4] = f14 - f12;
        fArr[i + 8] = f15 + f11;
        fArr[i + 1] = f14 + f12;
        fArr[i + 5] = 1.0f - (f18 + f13);
        fArr[i + 9] = f17 - f10;
        fArr[i + 2] = f15 - f11;
        fArr[i + 6] = f17 + f10;
        fArr[i + 10] = 1.0f - (f13 + f16);
    }

    public final String toString() {
        return String.format("[%.3f, %.3f, %.3f, %.3f]", new Object[]{Float.valueOf(this.x), Float.valueOf(this.y), Float.valueOf(this.z), Float.valueOf(this.w)});
    }

    public Quaternion(float f, float f2, float f3, float f4) {
        j(f, f2, f3, f4);
    }

    public Quaternion(Quaternion quaternion) {
        j(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}
