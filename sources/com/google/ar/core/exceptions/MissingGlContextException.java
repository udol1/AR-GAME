package com.google.ar.core.exceptions;

public class MissingGlContextException extends IllegalStateException {
    public MissingGlContextException() {
    }

    public MissingGlContextException(String str) {
        super(str);
    }
}
