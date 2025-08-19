package com.unity3d.plugin;

import android.app.Activity;
import android.app.FragmentTransaction;
import android.os.Build;
import android.os.Bundle;

public class UnityAndroidPermissions {

    interface IPermissionRequestResult {
        void OnPermissionDenied(String str);

        void OnPermissionGranted(String str);
    }

    public boolean IsPermissionGranted(Activity activity, String str) {
        if (Build.VERSION.SDK_INT < 23) {
            return true;
        }
        if (activity == null) {
            throw new NullPointerException("Invalid activity.");
        } else if (activity.checkSelfPermission(str) == 0) {
            return true;
        } else {
            return false;
        }
    }

    public void RequestPermissionAsync(Activity activity, String[] strArr, IPermissionRequestResult iPermissionRequestResult) {
        if (Build.VERSION.SDK_INT >= 23 && activity != null && strArr != null && iPermissionRequestResult != null) {
            PermissionFragment permissionFragment = new PermissionFragment(iPermissionRequestResult);
            Bundle bundle = new Bundle();
            bundle.putStringArray(PermissionFragment.PERMISSION_NAMES, strArr);
            permissionFragment.setArguments(bundle);
            FragmentTransaction beginTransaction = activity.getFragmentManager().beginTransaction();
            beginTransaction.add(0, permissionFragment);
            beginTransaction.commit();
        }
    }
}
