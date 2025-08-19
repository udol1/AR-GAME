package com.google.ar.core.exceptions;

public class SessionNotPausedException extends IllegalStateException {
    public SessionNotPausedException() {
    }

    public SessionNotPausedException(String str) {
        super(str);
    }
}
