package com.unity3d.player;

import java.lang.reflect.Method;
import java.util.HashMap;

final class n {

    /* renamed from: a  reason: collision with root package name */
    private HashMap f161a = new HashMap();

    /* renamed from: b  reason: collision with root package name */
    private Class f162b = null;
    private Object c = null;

    class a {

        /* renamed from: a  reason: collision with root package name */
        public Class[] f163a;

        /* renamed from: b  reason: collision with root package name */
        public Method f164b = null;

        public a(Class[] clsArr) {
            this.f163a = clsArr;
        }
    }

    public n(Class cls, Object obj) {
        this.f162b = cls;
        this.c = obj;
    }

    private void a(String str, a aVar) {
        try {
            aVar.f164b = this.f162b.getMethod(str, aVar.f163a);
        } catch (Exception e) {
            f.Log(6, "Exception while trying to get method " + str + ". " + e.getLocalizedMessage());
            aVar.f164b = null;
        }
    }

    public final Object a(String str, Object... objArr) {
        StringBuilder sb;
        if (!this.f161a.containsKey(str)) {
            sb = new StringBuilder("No definition for method ");
            sb.append(str);
            str = " can be found";
        } else {
            a aVar = (a) this.f161a.get(str);
            if (aVar.f164b == null) {
                a(str, aVar);
            }
            if (aVar.f164b == null) {
                sb = new StringBuilder("Unable to create method: ");
            } else {
                try {
                    return objArr.length == 0 ? aVar.f164b.invoke(this.c, new Object[0]) : aVar.f164b.invoke(this.c, objArr);
                } catch (Exception e) {
                    f.Log(6, "Error trying to call delegated method " + str + ". " + e.getLocalizedMessage());
                    return null;
                }
            }
        }
        sb.append(str);
        f.Log(6, sb.toString());
        return null;
    }

    public final void a(String str, Class[] clsArr) {
        this.f161a.put(str, new a(clsArr));
    }
}
