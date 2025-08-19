package com.google.ar.core.exceptions;

public class ResourceExhaustedException extends RuntimeException {
    public ResourceExhaustedException() {
    }

    public ResourceExhaustedException(String str) {
        super(str);
    }
}
