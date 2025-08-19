package com.google.ar.core;

import android.animation.ValueAnimator;

/* renamed from: com.google.ar.core.r  reason: case insensitive filesystem */
/* compiled from: InstallActivity */
final class C0001r implements ValueAnimator.AnimatorUpdateListener {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ int f34a;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ int f35b;
    final /* synthetic */ int c;
    final /* synthetic */ InstallActivity d;

    C0001r(InstallActivity installActivity, int i, int i2, int i3) {
        this.d = installActivity;
        this.f34a = i;
        this.f35b = i2;
        this.c = i3;
    }

    public final void onAnimationUpdate(ValueAnimator valueAnimator) {
        float animatedFraction = 1.0f - valueAnimator.getAnimatedFraction();
        float animatedFraction2 = valueAnimator.getAnimatedFraction();
        int i = this.f34a;
        float f = ((float) this.f35b) * animatedFraction2;
        this.d.getWindow().setLayout((int) ((((float) i) * animatedFraction) + f), (int) ((((float) this.c) * animatedFraction) + f));
        this.d.getWindow().getDecorView().refreshDrawableState();
    }
}
