package com.unity3d.player;

import android.content.Context;
import android.graphics.Rect;
import android.graphics.SurfaceTexture;
import android.hardware.camera2.CameraAccessException;
import android.hardware.camera2.CameraCaptureSession;
import android.hardware.camera2.CameraCharacteristics;
import android.hardware.camera2.CameraDevice;
import android.hardware.camera2.CameraManager;
import android.hardware.camera2.CaptureFailure;
import android.hardware.camera2.CaptureRequest;
import android.hardware.camera2.TotalCaptureResult;
import android.hardware.camera2.params.MeteringRectangle;
import android.hardware.camera2.params.StreamConfigurationMap;
import android.media.Image;
import android.media.ImageReader;
import android.os.Handler;
import android.os.HandlerThread;
import android.util.Range;
import android.util.Size;
import android.view.Surface;
import java.nio.ByteBuffer;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;

public final class a {

    /* renamed from: b  reason: collision with root package name */
    private static CameraManager f130b;
    private static String[] c;
    /* access modifiers changed from: private */
    public static Semaphore e = new Semaphore(1);
    private CameraCaptureSession.CaptureCallback A = new CameraCaptureSession.CaptureCallback() {
        public final void onCaptureCompleted(CameraCaptureSession cameraCaptureSession, CaptureRequest captureRequest, TotalCaptureResult totalCaptureResult) {
            a.this.a(captureRequest.getTag());
        }

        public final void onCaptureFailed(CameraCaptureSession cameraCaptureSession, CaptureRequest captureRequest, CaptureFailure captureFailure) {
            f.Log(5, "Camera2: Capture session failed " + captureRequest.getTag() + " reason " + captureFailure.getReason());
            a.this.a(captureRequest.getTag());
        }

        public final void onCaptureSequenceAborted(CameraCaptureSession cameraCaptureSession, int i) {
        }

        public final void onCaptureSequenceCompleted(CameraCaptureSession cameraCaptureSession, int i, long j) {
        }
    };
    private final CameraDevice.StateCallback B = new CameraDevice.StateCallback() {
        public final void onClosed(CameraDevice cameraDevice) {
            a.e.release();
        }

        public final void onDisconnected(CameraDevice cameraDevice) {
            f.Log(5, "Camera2: CameraDevice disconnected.");
            a.this.a(cameraDevice);
            a.e.release();
        }

        public final void onError(CameraDevice cameraDevice, int i) {
            f.Log(6, "Camera2: Error opeining CameraDevice " + i);
            a.this.a(cameraDevice);
            a.e.release();
        }

        public final void onOpened(CameraDevice cameraDevice) {
            CameraDevice unused = a.this.d = cameraDevice;
            a.e.release();
        }
    };
    private final ImageReader.OnImageAvailableListener C = new ImageReader.OnImageAvailableListener() {
        public final void onImageAvailable(ImageReader imageReader) {
            if (a.e.tryAcquire()) {
                Image acquireNextImage = imageReader.acquireNextImage();
                if (acquireNextImage != null) {
                    Image.Plane[] planes = acquireNextImage.getPlanes();
                    if (acquireNextImage.getFormat() == 35 && planes != null && planes.length == 3) {
                        c h = a.this.f131a;
                        ByteBuffer buffer = planes[0].getBuffer();
                        ByteBuffer buffer2 = planes[1].getBuffer();
                        ByteBuffer buffer3 = planes[2].getBuffer();
                        h.a(buffer, buffer2, buffer3, planes[0].getRowStride(), planes[1].getRowStride(), planes[1].getPixelStride());
                    } else {
                        f.Log(6, "Camera2: Wrong image format.");
                    }
                    if (a.this.s != null) {
                        a.this.s.close();
                    }
                    Image unused = a.this.s = acquireNextImage;
                }
                a.e.release();
            }
        }
    };
    private final SurfaceTexture.OnFrameAvailableListener D = new SurfaceTexture.OnFrameAvailableListener() {
        public final void onFrameAvailable(SurfaceTexture surfaceTexture) {
            a.this.f131a.a(surfaceTexture);
        }
    };
    /* access modifiers changed from: private */

    /* renamed from: a  reason: collision with root package name */
    public c f131a = null;
    /* access modifiers changed from: private */
    public CameraDevice d;
    private HandlerThread f;
    private Handler g;
    private Rect h;
    private Rect i;
    private int j;
    private int k;
    private float l = -1.0f;
    private float m = -1.0f;
    private int n;
    private int o;
    private boolean p = false;
    /* access modifiers changed from: private */
    public Range q;
    /* access modifiers changed from: private */
    public ImageReader r = null;
    /* access modifiers changed from: private */
    public Image s;
    /* access modifiers changed from: private */
    public CaptureRequest.Builder t;
    /* access modifiers changed from: private */
    public CameraCaptureSession u = null;
    /* access modifiers changed from: private */
    public Object v = new Object();
    private int w;
    private SurfaceTexture x;
    /* access modifiers changed from: private */
    public Surface y = null;
    private int z = C0000a.c;

    /* renamed from: com.unity3d.player.a$a  reason: collision with other inner class name */
    private enum C0000a {
        ;

        static {
            d = new int[]{1, 2, 3};
        }
    }

    protected a(c cVar) {
        this.f131a = cVar;
        g();
    }

    public static int a(Context context) {
        return c(context).length;
    }

    public static int a(Context context, int i2) {
        try {
            return ((Integer) b(context).getCameraCharacteristics(c(context)[i2]).get(CameraCharacteristics.SENSOR_ORIENTATION)).intValue();
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
            return 0;
        }
    }

    private static int a(Range[] rangeArr, int i2) {
        int i3 = -1;
        double d2 = Double.MAX_VALUE;
        for (int i4 = 0; i4 < rangeArr.length; i4++) {
            int intValue = ((Integer) rangeArr[i4].getLower()).intValue();
            int intValue2 = ((Integer) rangeArr[i4].getUpper()).intValue();
            float f2 = (float) i2;
            if (f2 + 0.1f > ((float) intValue) && f2 - 0.1f < ((float) intValue2)) {
                return i2;
            }
            double min = (double) ((float) Math.min(Math.abs(i2 - intValue), Math.abs(i2 - intValue2)));
            if (min < d2) {
                i3 = i4;
                d2 = min;
            }
        }
        return ((Integer) (i2 > ((Integer) rangeArr[i3].getUpper()).intValue() ? rangeArr[i3].getUpper() : rangeArr[i3].getLower())).intValue();
    }

    private static Rect a(Size[] sizeArr, double d2, double d3) {
        Size[] sizeArr2 = sizeArr;
        double d4 = Double.MAX_VALUE;
        int i2 = 0;
        int i3 = 0;
        for (int i4 = 0; i4 < sizeArr2.length; i4++) {
            int width = sizeArr2[i4].getWidth();
            int height = sizeArr2[i4].getHeight();
            double abs = Math.abs(Math.log(d2 / ((double) width))) + Math.abs(Math.log(d3 / ((double) height)));
            if (abs < d4) {
                i2 = width;
                i3 = height;
                d4 = abs;
            }
        }
        return new Rect(0, 0, i2, i3);
    }

    /* access modifiers changed from: private */
    public void a(CameraDevice cameraDevice) {
        synchronized (this.v) {
            this.u = null;
        }
        cameraDevice.close();
        this.d = null;
    }

    /* access modifiers changed from: private */
    public void a(Object obj) {
        if (obj == "Focus") {
            this.p = false;
            synchronized (this.v) {
                if (this.u != null) {
                    try {
                        this.t.set(CaptureRequest.CONTROL_AF_TRIGGER, 0);
                        this.t.setTag("Regular");
                        this.u.setRepeatingRequest(this.t.build(), this.A, this.g);
                    } catch (CameraAccessException e2) {
                        f.Log(6, "Camera2: CameraAccessException " + e2);
                    }
                }
            }
        } else if (obj == "Cancel focus") {
            synchronized (this.v) {
                if (this.u != null) {
                    j();
                }
            }
        }
    }

    private static Size[] a(CameraCharacteristics cameraCharacteristics) {
        String str;
        StreamConfigurationMap streamConfigurationMap = (StreamConfigurationMap) cameraCharacteristics.get(CameraCharacteristics.SCALER_STREAM_CONFIGURATION_MAP);
        if (streamConfigurationMap == null) {
            str = "Camera2: configuration map is not available.";
        } else {
            Size[] outputSizes = streamConfigurationMap.getOutputSizes(35);
            if (outputSizes != null && outputSizes.length != 0) {
                return outputSizes;
            }
            str = "Camera2: output sizes for YUV_420_888 format are not avialable.";
        }
        f.Log(6, str);
        return null;
    }

    private static CameraManager b(Context context) {
        if (f130b == null) {
            f130b = (CameraManager) context.getSystemService("camera");
        }
        return f130b;
    }

    private void b(CameraCharacteristics cameraCharacteristics) {
        int intValue = ((Integer) cameraCharacteristics.get(CameraCharacteristics.CONTROL_MAX_REGIONS_AF)).intValue();
        this.k = intValue;
        if (intValue > 0) {
            Rect rect = (Rect) cameraCharacteristics.get(CameraCharacteristics.SENSOR_INFO_ACTIVE_ARRAY_SIZE);
            this.i = rect;
            float width = ((float) rect.width()) / ((float) this.i.height());
            float width2 = ((float) this.h.width()) / ((float) this.h.height());
            if (width2 > width) {
                this.n = 0;
                this.o = (int) ((((float) this.i.height()) - (((float) this.i.width()) / width2)) / 2.0f);
            } else {
                this.o = 0;
                this.n = (int) ((((float) this.i.width()) - (((float) this.i.height()) * width2)) / 2.0f);
            }
            this.j = Math.min(this.i.width(), this.i.height()) / 20;
        }
    }

    public static boolean b(Context context, int i2) {
        try {
            return ((Integer) b(context).getCameraCharacteristics(c(context)[i2]).get(CameraCharacteristics.LENS_FACING)).intValue() == 0;
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
            return false;
        }
    }

    public static boolean c(Context context, int i2) {
        try {
            return ((Integer) b(context).getCameraCharacteristics(c(context)[i2]).get(CameraCharacteristics.CONTROL_MAX_REGIONS_AF)).intValue() > 0;
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
            return false;
        }
    }

    private static String[] c(Context context) {
        if (c == null) {
            try {
                c = b(context).getCameraIdList();
            } catch (CameraAccessException e2) {
                f.Log(6, "Camera2: CameraAccessException " + e2);
                c = new String[0];
            }
        }
        return c;
    }

    public static int[] d(Context context, int i2) {
        try {
            Size[] a2 = a(b(context).getCameraCharacteristics(c(context)[i2]));
            if (a2 == null) {
                return null;
            }
            int[] iArr = new int[(a2.length * 2)];
            for (int i3 = 0; i3 < a2.length; i3++) {
                int i4 = i3 * 2;
                iArr[i4] = a2[i3].getWidth();
                iArr[i4 + 1] = a2[i3].getHeight();
            }
            return iArr;
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
            return null;
        }
    }

    private void g() {
        HandlerThread handlerThread = new HandlerThread("CameraBackground");
        this.f = handlerThread;
        handlerThread.start();
        this.g = new Handler(this.f.getLooper());
    }

    private void h() {
        this.f.quit();
        try {
            this.f.join(4000);
            this.f = null;
            this.g = null;
        } catch (InterruptedException e2) {
            this.f.interrupt();
            f.Log(6, "Camera2: Interrupted while waiting for the background thread to finish " + e2);
        }
    }

    private void i() {
        try {
            if (!e.tryAcquire(4, TimeUnit.SECONDS)) {
                f.Log(5, "Camera2: Timeout waiting to lock camera for closing.");
                return;
            }
            this.d.close();
            try {
                if (!e.tryAcquire(4, TimeUnit.SECONDS)) {
                    f.Log(5, "Camera2: Timeout waiting to close camera.");
                }
            } catch (InterruptedException e2) {
                f.Log(6, "Camera2: Interrupted while waiting to close camera " + e2);
            }
            this.d = null;
            e.release();
        } catch (InterruptedException e3) {
            f.Log(6, "Camera2: Interrupted while trying to lock camera for closing " + e3);
        }
    }

    /* access modifiers changed from: private */
    public void j() {
        try {
            if (this.k != 0 && this.l >= 0.0f && this.l <= 1.0f && this.m >= 0.0f) {
                if (this.m <= 1.0f) {
                    this.p = true;
                    int max = Math.max(this.j + 1, Math.min((int) ((((float) (this.i.width() - (this.n * 2))) * this.l) + ((float) this.n)), (this.i.width() - this.j) - 1));
                    int max2 = Math.max(this.j + 1, Math.min((int) ((((double) (this.i.height() - (this.o * 2))) * (1.0d - ((double) this.m))) + ((double) this.o)), (this.i.height() - this.j) - 1));
                    this.t.set(CaptureRequest.CONTROL_AF_REGIONS, new MeteringRectangle[]{new MeteringRectangle(max - this.j, max2 - this.j, this.j * 2, this.j * 2, 999)});
                    this.t.set(CaptureRequest.CONTROL_AF_MODE, 1);
                    this.t.set(CaptureRequest.CONTROL_AF_TRIGGER, 1);
                    this.t.setTag("Focus");
                    this.u.capture(this.t.build(), this.A, this.g);
                    return;
                }
            }
            this.t.set(CaptureRequest.CONTROL_AF_MODE, 4);
            this.t.setTag("Regular");
            if (this.u != null) {
                this.u.setRepeatingRequest(this.t.build(), this.A, this.g);
            }
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
        }
    }

    private void k() {
        try {
            if (this.u != null) {
                this.u.stopRepeating();
                this.t.set(CaptureRequest.CONTROL_AF_TRIGGER, 2);
                this.t.set(CaptureRequest.CONTROL_AF_MODE, 0);
                this.t.setTag("Cancel focus");
                this.u.capture(this.t.build(), this.A, this.g);
            }
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
        }
    }

    public final Rect a() {
        return this.h;
    }

    public final boolean a(float f2, float f3) {
        if (this.k <= 0) {
            return false;
        }
        if (!this.p) {
            this.l = f2;
            this.m = f3;
            synchronized (this.v) {
                if (!(this.u == null || this.z == C0000a.f138b)) {
                    k();
                }
            }
            return true;
        }
        f.Log(5, "Camera2: Setting manual focus point already started.");
        return false;
    }

    public final boolean a(Context context, int i2, int i3, int i4, int i5, int i6) {
        try {
            CameraCharacteristics cameraCharacteristics = f130b.getCameraCharacteristics(c(context)[i2]);
            if (((Integer) cameraCharacteristics.get(CameraCharacteristics.INFO_SUPPORTED_HARDWARE_LEVEL)).intValue() == 2) {
                f.Log(5, "Camera2: only LEGACY hardware level is supported.");
                return false;
            }
            Size[] a2 = a(cameraCharacteristics);
            if (!(a2 == null || a2.length == 0)) {
                this.h = a(a2, (double) i3, (double) i4);
                Range[] rangeArr = (Range[]) cameraCharacteristics.get(CameraCharacteristics.CONTROL_AE_AVAILABLE_TARGET_FPS_RANGES);
                if (rangeArr == null || rangeArr.length == 0) {
                    f.Log(6, "Camera2: target FPS ranges are not avialable.");
                } else {
                    int a3 = a(rangeArr, i5);
                    this.q = new Range(Integer.valueOf(a3), Integer.valueOf(a3));
                    try {
                        if (!e.tryAcquire(4, TimeUnit.SECONDS)) {
                            f.Log(5, "Camera2: Timeout waiting to lock camera for opening.");
                            return false;
                        }
                        try {
                            f130b.openCamera(c(context)[i2], this.B, this.g);
                            try {
                                if (!e.tryAcquire(4, TimeUnit.SECONDS)) {
                                    f.Log(5, "Camera2: Timeout waiting to open camera.");
                                    return false;
                                }
                                e.release();
                                this.w = i6;
                                b(cameraCharacteristics);
                                return this.d != null;
                            } catch (InterruptedException e2) {
                                f.Log(6, "Camera2: Interrupted while waiting to open camera " + e2);
                            }
                        } catch (CameraAccessException e3) {
                            f.Log(6, "Camera2: CameraAccessException " + e3);
                            e.release();
                            return false;
                        }
                    } catch (InterruptedException e4) {
                        f.Log(6, "Camera2: Interrupted while trying to lock camera for opening " + e4);
                        return false;
                    }
                }
            }
            return false;
        } catch (CameraAccessException e5) {
            f.Log(6, "Camera2: CameraAccessException " + e5);
            return false;
        }
    }

    public final void b() {
        if (this.d != null) {
            e();
            i();
            this.A = null;
            this.y = null;
            this.x = null;
            Image image = this.s;
            if (image != null) {
                image.close();
                this.s = null;
            }
            ImageReader imageReader = this.r;
            if (imageReader != null) {
                imageReader.close();
                this.r = null;
            }
        }
        h();
    }

    public final void c() {
        List list;
        if (this.r == null) {
            ImageReader newInstance = ImageReader.newInstance(this.h.width(), this.h.height(), 35, 2);
            this.r = newInstance;
            newInstance.setOnImageAvailableListener(this.C, this.g);
            this.s = null;
            if (this.w != 0) {
                SurfaceTexture surfaceTexture = new SurfaceTexture(this.w);
                this.x = surfaceTexture;
                surfaceTexture.setDefaultBufferSize(this.h.width(), this.h.height());
                this.x.setOnFrameAvailableListener(this.D, this.g);
                this.y = new Surface(this.x);
            }
        }
        try {
            if (this.u == null) {
                CameraDevice cameraDevice = this.d;
                if (this.y != null) {
                    list = Arrays.asList(new Surface[]{this.y, this.r.getSurface()});
                } else {
                    list = Arrays.asList(new Surface[]{this.r.getSurface()});
                }
                cameraDevice.createCaptureSession(list, new CameraCaptureSession.StateCallback() {
                    public final void onConfigureFailed(CameraCaptureSession cameraCaptureSession) {
                        f.Log(6, "Camera2: CaptureSession configuration failed.");
                    }

                    public final void onConfigured(CameraCaptureSession cameraCaptureSession) {
                        if (a.this.d != null) {
                            synchronized (a.this.v) {
                                CameraCaptureSession unused = a.this.u = cameraCaptureSession;
                                try {
                                    CaptureRequest.Builder unused2 = a.this.t = a.this.d.createCaptureRequest(1);
                                    if (a.this.y != null) {
                                        a.this.t.addTarget(a.this.y);
                                    }
                                    a.this.t.addTarget(a.this.r.getSurface());
                                    a.this.t.set(CaptureRequest.CONTROL_AE_TARGET_FPS_RANGE, a.this.q);
                                    a.this.j();
                                } catch (CameraAccessException e) {
                                    f.Log(6, "Camera2: CameraAccessException " + e);
                                }
                            }
                        }
                    }
                }, this.g);
            } else if (this.z == C0000a.f138b) {
                this.u.setRepeatingRequest(this.t.build(), this.A, this.g);
            }
            this.z = C0000a.f137a;
        } catch (CameraAccessException e2) {
            f.Log(6, "Camera2: CameraAccessException " + e2);
        }
    }

    public final void d() {
        synchronized (this.v) {
            if (this.u != null) {
                try {
                    this.u.stopRepeating();
                    this.z = C0000a.f138b;
                } catch (CameraAccessException e2) {
                    f.Log(6, "Camera2: CameraAccessException " + e2);
                }
            }
        }
    }

    public final void e() {
        synchronized (this.v) {
            if (this.u != null) {
                try {
                    this.u.abortCaptures();
                } catch (CameraAccessException e2) {
                    f.Log(6, "Camera2: CameraAccessException " + e2);
                }
                this.u.close();
                this.u = null;
                this.z = C0000a.c;
            }
        }
    }
}
