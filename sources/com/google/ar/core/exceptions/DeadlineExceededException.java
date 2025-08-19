package com.google.ar.core.exceptions;

public class DeadlineExceededException extends IllegalStateException {
    public DeadlineExceededException() {
    }

    public DeadlineExceededException(String str) {
        super(str);
    }
}
