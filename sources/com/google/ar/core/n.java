package com.google.ar.core;

import java.util.Map;

/* compiled from: FaceCache */
final class n {

    /* renamed from: a  reason: collision with root package name */
    final Map<Long, AugmentedFace> f28a = new m();

    n() {
    }

    public final synchronized AugmentedFace a(long j, Session session) {
        Map<Long, AugmentedFace> map = this.f28a;
        Long valueOf = Long.valueOf(j);
        AugmentedFace augmentedFace = map.get(valueOf);
        if (augmentedFace != null) {
            return augmentedFace;
        }
        AugmentedFace augmentedFace2 = new AugmentedFace(j, session);
        this.f28a.put(valueOf, augmentedFace2);
        return augmentedFace2;
    }
}
