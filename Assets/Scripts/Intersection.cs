using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public static bool InterSegPlane(Segment segment, Plane plane, out Vector3 interP, out Vector3 vectN)
    {
        interP = new Vector3(); vectN = new Vector3();
        float dot = Vector3.Dot(segment.P2 - segment.P1, plane.N);

        if (Mathf.Approximately(0, dot))
        {
            return false;
            /*
            // S'ils sont parallèles
            bool inter = plane.N.x * segment.P1.x + plane.N.y * segment.P1.y + plane.N.z * segment.P1.z == plane.D;
            if (inter)
            {
                // S'ils sont confondus
                interP = segment.P1;
            }
            vectN = plane.N;

            return inter;*/
        }

        float t = (plane.D - Vector3.Dot(segment.P1, plane.N)) / (dot);

        if (!(0 <= t && t <= 1))
        {
            // S'ils ne sont pas sécants
            return false;
        }
        else
        {
            // S'ils sont sécants
            interP = segment.P1 + t * (segment.P2 - segment.P1);
            vectN = plane.N;
            if (dot < 0)
            {
                vectN *= -1;
            }
            return true;
        }
    }

    public static bool InterSegSphere(Segment segment, Sphere sphere, out Vector3 interP, out Vector3 vectN)
    {
        interP = new Vector3(); vectN = new Vector3();
        return false;
    }
}