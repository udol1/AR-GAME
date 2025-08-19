package com.google.ar.core.exceptions;

public class SessionUnsupportedException extends IllegalStateException {
    public SessionUnsupportedException() {
    }

    public SessionUnsupportedException(String str) {
        super(str);
    }
}
