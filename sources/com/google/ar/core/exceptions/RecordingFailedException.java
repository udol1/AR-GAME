package com.google.ar.core.exceptions;

import java.io.IOException;

public class RecordingFailedException extends IOException {
    public RecordingFailedException() {
    }

    public RecordingFailedException(String str) {
        super(str);
    }
}
