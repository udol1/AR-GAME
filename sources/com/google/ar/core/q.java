package com.google.ar.core;

import android.view.View;
import com.google.ar.core.exceptions.UnavailableUserDeclinedInstallationException;

/* compiled from: InstallActivity */
final class q implements View.OnClickListener {

    /* renamed from: a  reason: collision with root package name */
    final /* synthetic */ InstallActivity f32a;

    /* renamed from: b  reason: collision with root package name */
    private final /* synthetic */ int f33b;

    q(InstallActivity installActivity) {
        this.f32a = installActivity;
    }

    q(InstallActivity installActivity, int i) {
        this.f33b = i;
        this.f32a = installActivity;
    }

    public final void onClick(View view) {
        if (this.f33b != 0) {
            this.f32a.finishWithFailure(new UnavailableUserDeclinedInstallationException());
            return;
        }
        this.f32a.animateToSpinner();
        this.f32a.startInstaller();
    }
}
