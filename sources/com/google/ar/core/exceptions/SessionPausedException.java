package com.google.ar.core.exceptions;

public class SessionPausedException extends IllegalStateException {
    public SessionPausedException() {
    }

    public SessionPausedException(String str) {
        super(str);
    }
}
