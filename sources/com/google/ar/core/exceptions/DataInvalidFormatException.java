package com.google.ar.core.exceptions;

public class DataInvalidFormatException extends IllegalArgumentException {
    public DataInvalidFormatException() {
    }

    public DataInvalidFormatException(String str) {
        super(str);
    }
}
