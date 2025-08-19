package com.unity3d.unitygar;

import android.app.Activity;
import com.google.atap.tangoservice.Tango;
import com.google.atap.tangoservice.TangoCameraMetadata;
import com.google.atap.tangoservice.TangoEvent;
import com.google.atap.tangoservice.TangoImage;
import com.google.atap.tangoservice.TangoPointCloudData;
import com.google.atap.tangoservice.TangoPoseData;

public class GoogleAR {
    private final native void tangoCacheTangoObject(Tango tango);

    private final native void tangoOnCreate(Activity activity, Tango.TangoUpdateCallback tangoUpdateCallback);

    private final native void tangoOnImageAvailable(TangoImage tangoImage, TangoCameraMetadata tangoCameraMetadata, int i);

    private final native void tangoOnPause();

    private final native void tangoOnPointCloudAvailable(TangoPointCloudData tangoPointCloudData);

    private final native void tangoOnPoseAvailable(TangoPoseData tangoPoseData);

    private final native void tangoOnTangoEvent(TangoEvent tangoEvent);

    private final native void tangoOnTextureAvailable(int i);

    public int getClassVersion() {
        return 1;
    }
}
