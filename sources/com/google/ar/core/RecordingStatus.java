package com.google.ar.core;

import com.google.ar.core.exceptions.FatalException;

public enum RecordingStatus {
    NONE(0),
    OK(1),
    IO_ERROR(2);
    
    final int nativeCode;

    private RecordingStatus(int i) {
        this.nativeCode = i;
    }

    static RecordingStatus forNumber(int i) {
        for (RecordingStatus recordingStatus : values()) {
            if (recordingStatus.nativeCode == i) {
                return recordingStatus;
            }
        }
        StringBuilder sb = new StringBuilder(62);
        sb.append("Unexpected value for native RecordingStatus, value=");
        sb.append(i);
        throw new FatalException(sb.toString());
    }
}
