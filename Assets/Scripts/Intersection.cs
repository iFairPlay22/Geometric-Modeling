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

        Vector3 omegaA = segment.P1 - sphere.P;
        Vector3 ab = segment.P2 - segment.P1;

        // On calcule delta
        float a = Mathf.Pow(Vector3.Magnitude(ab), 2);
        float b = 2 * (Vector3.Dot(omegaA, ab));
        float c = Mathf.Pow(Vector3.Magnitude(omegaA), 2) - Mathf.Pow(sphere.R, 2);
        float delta = Mathf.Pow(b, 2) - 4 * a * c;

        if (delta < 0)
        {
            // Pas d'intersections
            return false;
        }

        // On calcule les valeurs t1 et t2
        float t1 = (-b - Mathf.Sqrt(delta)) / (2 * a);
        float t2 = (-b + Mathf.Sqrt(delta)) / (2 * a);

        // On retourne les valeurs d'intersection
        bool t1_01 = 0 <= t1 && t1 <= 1;
        bool t2_01 = 0 <= t2 && t2 <= 1;
        float t;

        if (t1_01 && t2_01)
        {
            t = Mathf.Min(t1, t2);
        }
        else if (t1_01)
        {
            t = t1;
        }
        else if (t2_01)
        {
            t = t2;
        }
        else
        {
            return false;
        }

        interP = segment.P1 + t * (segment.P2 - segment.P1);
        vectN = Vector3.Normalize(interP - sphere.P);

        return true;
    }

    public static bool InterSegCylinder(Segment segment, Cylinder cylinder, out Vector3 interP, out Vector3 vectN)
    {
        interP = new Vector3(); vectN = new Vector3();
        return false;
    }
}