package com.unity3d.player;

import java.util.concurrent.atomic.AtomicReference;

public class GoogleVrApi {

    /* renamed from: a  reason: collision with root package name */
    private static AtomicReference f57a = new AtomicReference();

    private GoogleVrApi() {
    }

    static void a() {
        f57a.set((Object) null);
    }

    static void a(e eVar) {
        f57a.compareAndSet((Object) null, new GoogleVrProxy(eVar));
    }

    static GoogleVrProxy b() {
        return (GoogleVrProxy) f57a.get();
    }

    public static GoogleVrVideo getGoogleVrVideo() {
        return (GoogleVrVideo) f57a.get();
    }
}
