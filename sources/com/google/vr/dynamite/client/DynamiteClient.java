package com.google.vr.dynamite.client;

import android.content.Context;
import android.os.RemoteException;
import android.util.ArrayMap;
import android.util.Log;
import dalvik.system.DexClassLoader;

public final class DynamiteClient {

    /* renamed from: a  reason: collision with root package name */
    private static final ArrayMap<g, e> f49a = new ArrayMap<>();

    private DynamiteClient() {
    }

    public static synchronized int checkVersion(Context context, String str, String str2, String str3) {
        synchronized (DynamiteClient.class) {
            g gVar = new g(str, str2);
            e remoteLibraryLoaderFromInfo = getRemoteLibraryLoaderFromInfo(gVar);
            try {
                INativeLibraryLoader newNativeLibraryLoader = remoteLibraryLoaderFromInfo.b(context).newNativeLibraryLoader(ObjectWrapper.b(remoteLibraryLoaderFromInfo.a(context)), ObjectWrapper.b(context));
                if (newNativeLibraryLoader == null) {
                    String valueOf = String.valueOf(gVar);
                    StringBuilder sb = new StringBuilder(String.valueOf(valueOf).length() + 72);
                    sb.append("Failed to load native library ");
                    sb.append(valueOf);
                    sb.append(" from remote package: no loader available.");
                    Log.e("DynamiteClient", sb.toString());
                    return -1;
                }
                int checkVersion = newNativeLibraryLoader.checkVersion(str3);
                return checkVersion;
            } catch (RemoteException | d | IllegalArgumentException | IllegalStateException | SecurityException | UnsatisfiedLinkError e) {
                String valueOf2 = String.valueOf(gVar);
                StringBuilder sb2 = new StringBuilder(String.valueOf(valueOf2).length() + 54);
                sb2.append("Failed to load native library ");
                sb2.append(valueOf2);
                sb2.append(" from remote package:\n  ");
                Log.e("DynamiteClient", sb2.toString(), e);
                return -1;
            }
        }
    }

    public static synchronized ClassLoader getRemoteClassLoader(Context context, String str, String str2) {
        synchronized (DynamiteClient.class) {
            Context remoteContext = getRemoteContext(context, str, str2);
            if (remoteContext == null) {
                return null;
            }
            ClassLoader classLoader = remoteContext.getClassLoader();
            return classLoader;
        }
    }

    public static synchronized Context getRemoteContext(Context context, String str, String str2) {
        Context a2;
        synchronized (DynamiteClient.class) {
            g gVar = new g(str, str2);
            try {
                a2 = getRemoteLibraryLoaderFromInfo(gVar).a(context);
            } catch (d e) {
                String valueOf = String.valueOf(gVar);
                StringBuilder sb = new StringBuilder(String.valueOf(valueOf).length() + 52);
                sb.append("Failed to get remote Context");
                sb.append(valueOf);
                sb.append(" from remote package:\n  ");
                Log.e("DynamiteClient", sb.toString(), e);
                return null;
            }
        }
        return a2;
    }

    public static synchronized ClassLoader getRemoteDexClassLoader(Context context, String str) {
        synchronized (DynamiteClient.class) {
            Context remoteContext = getRemoteContext(context, str, (String) null);
            if (remoteContext == null) {
                return null;
            }
            try {
                DexClassLoader dexClassLoader = new DexClassLoader(remoteContext.getPackageCodePath(), context.getCodeCacheDir().getAbsolutePath(), remoteContext.getApplicationInfo().nativeLibraryDir, context.getClassLoader());
                return dexClassLoader;
            } catch (RuntimeException e) {
                Log.e("DynamiteClient", "Failed to create class loader for remote package\n ", e);
                return null;
            }
        }
    }

    private static synchronized e getRemoteLibraryLoaderFromInfo(g gVar) {
        synchronized (DynamiteClient.class) {
            e eVar = f49a.get(gVar);
            if (eVar != null) {
                return eVar;
            }
            e eVar2 = new e(gVar);
            f49a.put(gVar, eVar2);
            return eVar2;
        }
    }

    public static synchronized long loadNativeRemoteLibrary(Context context, String str, String str2) {
        synchronized (DynamiteClient.class) {
            g gVar = new g(str, str2);
            e remoteLibraryLoaderFromInfo = getRemoteLibraryLoaderFromInfo(gVar);
            try {
                INativeLibraryLoader newNativeLibraryLoader = remoteLibraryLoaderFromInfo.b(context).newNativeLibraryLoader(ObjectWrapper.b(remoteLibraryLoaderFromInfo.a(context)), ObjectWrapper.b(context));
                if (newNativeLibraryLoader == null) {
                    String valueOf = String.valueOf(gVar);
                    StringBuilder sb = new StringBuilder(String.valueOf(valueOf).length() + 72);
                    sb.append("Failed to load native library ");
                    sb.append(valueOf);
                    sb.append(" from remote package: no loader available.");
                    Log.e("DynamiteClient", sb.toString());
                    return 0;
                }
                long initializeAndLoadNativeLibrary = newNativeLibraryLoader.initializeAndLoadNativeLibrary(str2);
                return initializeAndLoadNativeLibrary;
            } catch (RemoteException | d | IllegalArgumentException | IllegalStateException | SecurityException | UnsatisfiedLinkError e) {
                String valueOf2 = String.valueOf(gVar);
                StringBuilder sb2 = new StringBuilder(String.valueOf(valueOf2).length() + 54);
                sb2.append("Failed to load native library ");
                sb2.append(valueOf2);
                sb2.append(" from remote package:\n  ");
                Log.e("DynamiteClient", sb2.toString(), e);
                return 0;
            }
        }
    }
}
