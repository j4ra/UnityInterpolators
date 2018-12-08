using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpolators;

public class SmoothMover1D : MonoBehaviour {

    public enum Mode  {
        SmoothStart2, SmoothStart3, SmoothStart4,
        SmoothStop2, SmoothStop3, SmoothStop4,
        SmoothStep3, SmoothStep5, SmoothStep7,
        Arch2, Arch4, Arch6
    };

    public Mode mode = Mode.SmoothStart2;
    public float min = -5;
    public float max = 5;
    public float period = 5;

    private float t = 0;
	void Update () {
        t += Time.deltaTime / period;
        if (t >= 1) t = 0;

        float p = getInterpolated(t);
        float x = Interpolator1D.Lerp(p, min, max);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

    private float getInterpolated(float t)
    {
        switch (mode)
        {
            case Mode.SmoothStart2:
                return Interpolator1D.SmoothStart2(t);
            case Mode.SmoothStart3:
                return Interpolator1D.SmoothStart3(t);
            case Mode.SmoothStart4:
                return Interpolator1D.SmoothStart4(t);
            case Mode.SmoothStop2:
                return Interpolator1D.SmoothStop2(t);
            case Mode.SmoothStop3:
                return Interpolator1D.SmoothStop3(t);
            case Mode.SmoothStop4:
                return Interpolator1D.SmoothStop4(t);
            case Mode.SmoothStep3:
                return Interpolator1D.SmoothStep3(t);
            case Mode.SmoothStep5:
                return Interpolator1D.SmoothStep5(t);
            case Mode.SmoothStep7:
                return Interpolator1D.SmoothStep7(t);
            case Mode.Arch2:
                return Interpolator1D.Arch2(t);
            case Mode.Arch4:
                return Interpolator1D.Arch4(t);
            case Mode.Arch6:
                return Interpolator1D.Arch6(t);
        }
        return 0;
    }
}
