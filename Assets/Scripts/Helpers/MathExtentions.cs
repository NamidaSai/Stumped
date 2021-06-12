using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtentions
{
    public static float V2InverseLerp(Vector2 a,Vector2 b,Vector2 value)
    {
        Vector2 AB = a - b;
        Vector2 AV = value - a;
        return Vector2.Dot(AV, AB) / (AB.magnitude * AB.magnitude);
    }
}
