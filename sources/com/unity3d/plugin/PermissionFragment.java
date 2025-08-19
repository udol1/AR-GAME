package com.unity3d.plugin;

import android.app.Fragment;
import android.app.FragmentTransaction;
import android.os.Bundle;
import com.unity3d.plugin.UnityAndroidPermissions;

public class PermissionFragment extends Fragment {
    private static final int PERMISSIONS_REQUEST_CODE = 15887;
    public static final String PERMISSION_NAMES = "PermissionNames";
    private final UnityAndroidPermissions.IPermissionRequestResult resultCallbacks;

    public PermissionFragment() {
        this.resultCallbacks = null;
    }

    public PermissionFragment(UnityAndroidPermissions.IPermissionRequestResult iPermissionRequestResult) {
        this.resultCallbacks = iPermissionRequestResult;
    }

    public void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        if (this.resultCallbacks == null) {
            getFragmentManager().beginTransaction().remove(this).commit();
        } else {
            requestPermissions(getArguments().getStringArray(PERMISSION_NAMES), PERMISSIONS_REQUEST_CODE);
        }
    }

    public void onRequestPermissionsResult(int i, String[] strArr, int[] iArr) {
        if (i == PERMISSIONS_REQUEST_CODE) {
            int i2 = 0;
            while (i2 < strArr.length && i2 < iArr.length) {
                if (iArr[i2] == 0) {
                    this.resultCallbacks.OnPermissionGranted(strArr[i2]);
                } else {
                    this.resultCallbacks.OnPermissionDenied(strArr[i2]);
                }
                i2++;
            }
            FragmentTransaction beginTransaction = getFragmentManager().beginTransaction();
            beginTransaction.remove(this);
            beginTransaction.commit();
        }
    }
}
