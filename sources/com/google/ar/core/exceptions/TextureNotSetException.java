package com.google.ar.core.exceptions;

public class TextureNotSetException extends IllegalStateException {
    public TextureNotSetException() {
    }

    public TextureNotSetException(String str) {
        super(str);
    }
}
