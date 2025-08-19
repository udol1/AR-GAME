package com.google.ar.core;

import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;

/* compiled from: InstallActivity */
final class s extends AnimatorListenerAdapter {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ InstallActivity f36a;

    s(InstallActivity installActivity) {
        this.f36a = installActivity;
    }

    public final void onAnimationEnd(Animator animator) {
        this.f36a.showSpinner();
    }
}
