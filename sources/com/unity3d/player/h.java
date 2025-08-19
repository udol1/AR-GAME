package com.unity3d.player;

import android.app.Fragment;
import android.app.FragmentTransaction;
import android.os.Bundle;
import com.unity3d.plugin.PermissionFragment;

public final class h extends Fragment {
    public final void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        requestPermissions(new String[]{getArguments().getString(PermissionFragment.PERMISSION_NAMES)}, 96489);
    }

    public final void onRequestPermissionsResult(int i, String[] strArr, int[] iArr) {
        if (i == 96489) {
            if (strArr.length == 0) {
                requestPermissions(new String[]{getArguments().getString(PermissionFragment.PERMISSION_NAMES)}, 96489);
                return;
            }
            FragmentTransaction beginTransaction = getActivity().getFragmentManager().beginTransaction();
            beginTransaction.remove(this);
            beginTransaction.commit();
        }
    }
}
