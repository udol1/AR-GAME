package com.google.ar.core.exceptions;

import java.io.IOException;

public class PlaybackFailedException extends IOException {
    public PlaybackFailedException() {
    }

    public PlaybackFailedException(String str) {
        super(str);
    }
}
