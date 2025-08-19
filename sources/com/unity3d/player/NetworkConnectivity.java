package com.unity3d.player;

import android.app.Activity;
import android.content.Context;
import android.net.ConnectivityManager;
import android.net.Network;
import android.net.NetworkCapabilities;

public class NetworkConnectivity extends Activity {

    /* renamed from: a  reason: collision with root package name */
    private final int f71a = 0;

    /* renamed from: b  reason: collision with root package name */
    private final int f72b = 1;
    private final int c = 2;
    /* access modifiers changed from: private */
    public int d = 0;
    private ConnectivityManager e;
    private final ConnectivityManager.NetworkCallback f = new ConnectivityManager.NetworkCallback() {
        public final void onAvailable(Network network) {
            super.onAvailable(network);
        }

        public final void onCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities) {
            NetworkConnectivity networkConnectivity;
            int i;
            super.onCapabilitiesChanged(network, networkCapabilities);
            if (networkCapabilities.hasTransport(0)) {
                networkConnectivity = NetworkConnectivity.this;
                i = 1;
            } else {
                networkConnectivity = NetworkConnectivity.this;
                i = 2;
            }
            int unused = networkConnectivity.d = i;
        }

        public final void onLost(Network network) {
            super.onLost(network);
            int unused = NetworkConnectivity.this.d = 0;
        }

        public final void onUnavailable() {
            super.onUnavailable();
            int unused = NetworkConnectivity.this.d = 0;
        }
    };

    public NetworkConnectivity(Context context) {
        ConnectivityManager connectivityManager = (ConnectivityManager) context.getSystemService("connectivity");
        this.e = connectivityManager;
        connectivityManager.registerDefaultNetworkCallback(this.f);
    }

    public final int a() {
        return this.d;
    }

    public final void b() {
        this.e.unregisterNetworkCallback(this.f);
    }
}
