package com.google.ar.core;

import com.google.ar.core.ArCoreApk;
import com.google.ar.core.exceptions.UnavailableUserDeclinedInstallationException;

/* compiled from: InstallActivity */
final class t {

    /* renamed from: a  reason: collision with root package name */
    boolean f37a = false;

    /* renamed from: b  reason: collision with root package name */
    final /* synthetic */ InstallActivity f38b;

    t(InstallActivity installActivity) {
        this.f38b = installActivity;
    }

    public final void a(u uVar) {
        synchronized (this.f38b) {
            if (!this.f37a) {
                u unused = this.f38b.lastEvent = uVar;
                u uVar2 = u.ACCEPTED;
                ArCoreApk.UserMessageType userMessageType = ArCoreApk.UserMessageType.APPLICATION;
                ArCoreApk.Availability availability = ArCoreApk.Availability.UNKNOWN_ERROR;
                int ordinal = uVar.ordinal();
                if (ordinal != 0) {
                    if (ordinal == 1) {
                        this.f38b.finishWithFailure(new UnavailableUserDeclinedInstallationException());
                    } else if (ordinal == 2) {
                        if (!this.f38b.waitingForCompletion && k.a().f25b) {
                            this.f38b.closeInstaller();
                        }
                        this.f38b.finishWithFailure((Exception) null);
                    }
                    this.f37a = true;
                }
            }
        }
    }

    public final void b(Exception exc) {
        synchronized (this.f38b) {
            if (!this.f37a) {
                this.f37a = true;
                u unused = this.f38b.lastEvent = u.CANCELLED;
                this.f38b.finishWithFailure(exc);
            }
        }
    }
}
