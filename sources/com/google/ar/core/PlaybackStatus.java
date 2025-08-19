package com.google.ar.core;

import com.google.ar.core.exceptions.FatalException;

public enum PlaybackStatus {
    NONE(0),
    OK(1),
    IO_ERROR(2),
    FINISHED(3);
    
    final int nativeCode;

    private PlaybackStatus(int i) {
        this.nativeCode = i;
    }

    static PlaybackStatus forNumber(int i) {
        for (PlaybackStatus playbackStatus : values()) {
            if (playbackStatus.nativeCode == i) {
                return playbackStatus;
            }
        }
        StringBuilder sb = new StringBuilder(61);
        sb.append("Unexpected value for native PlaybackStatus, value=");
        sb.append(i);
        throw new FatalException(sb.toString());
    }
}
