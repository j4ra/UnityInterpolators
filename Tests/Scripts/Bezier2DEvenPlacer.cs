using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curves;

[RequireComponent(typeof(Bezier2DCreator))]
public class Bezier2DEvenPlacer : MonoBehaviour {

    public int Resolution = 1;
    public float Distance = 0.1f;

    Bezier2DCreator Path;

	void Start () {
        Path = GetComponent<Bezier2DCreator>();
        Vector2[] points = Path.path.CalculateEvenlySpacedPoints(Distance, Resolution);
        foreach(var v in points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            g.transform.position = transform.TransformPoint(v);
            g.transform.localScale = Vector3.one * Distance;
        }
	}
}
