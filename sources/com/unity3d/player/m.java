package com.unity3d.player;

final class m {

    /* renamed from: a  reason: collision with root package name */
    private static boolean f159a = false;

    /* renamed from: b  reason: collision with root package name */
    private boolean f160b = false;
    private boolean c = false;
    private boolean d = true;
    private boolean e = false;

    m() {
    }

    static void a() {
        f159a = true;
    }

    static void b() {
        f159a = false;
    }

    static boolean c() {
        return f159a;
    }

    /* access modifiers changed from: package-private */
    public final void a(boolean z) {
        this.f160b = z;
    }

    /* access modifiers changed from: package-private */
    public final void b(boolean z) {
        this.d = z;
    }

    /* access modifiers changed from: package-private */
    public final void c(boolean z) {
        this.e = z;
    }

    /* access modifiers changed from: package-private */
    public final void d(boolean z) {
        this.c = z;
    }

    /* access modifiers changed from: package-private */
    public final boolean d() {
        return this.d;
    }

    /* access modifiers changed from: package-private */
    public final boolean e() {
        return this.e;
    }

    /* access modifiers changed from: package-private */
    public final boolean f() {
        return f159a && this.f160b && !this.d && !this.c;
    }

    /* access modifiers changed from: package-private */
    public final boolean g() {
        return this.c;
    }

    public final String toString() {
        return super.toString();
    }
}
