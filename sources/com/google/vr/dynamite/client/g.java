package com.google.vr.dynamite.client;

/* compiled from: TargetLibraryInfo */
final class g {

    /* renamed from: a  reason: collision with root package name */
    private final String f53a;

    /* renamed from: b  reason: collision with root package name */
    private final String f54b;

    public g(String str, String str2) {
        this.f53a = str;
        this.f54b = str2;
    }

    public final String a() {
        return this.f53a;
    }

    public final boolean equals(Object obj) {
        if (obj == this) {
            return true;
        }
        if (obj instanceof g) {
            g gVar = (g) obj;
            return f.a(this.f53a, gVar.f53a) && f.a(this.f54b, gVar.f54b);
        }
    }

    public final int hashCode() {
        return (f.b(this.f53a) * 37) + f.b(this.f54b);
    }

    public final String toString() {
        return "[packageName=" + this.f53a + ",libraryName=" + this.f54b + "]";
    }
}
