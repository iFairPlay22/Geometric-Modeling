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
            vectN = Vector3.Normalize(plane.N + interP);
            if (dot < 0)
            {
                vectN *= -1.0f;
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
        float b = 2.0f * (Vector3.Dot(omegaA, ab));
        float c = Mathf.Pow(Vector3.Magnitude(omegaA), 2) - Mathf.Pow(sphere.R, 2);
        float delta = b * b - 4.0f * a * c;

        if (delta < 0)
        {
            // Pas d'intersections
            return false;
        }

        // On calcule les valeurs t1 et t2
        float t1 = (-b - Mathf.Sqrt(delta)) / (2.0f * a);
        float t2 = (-b + Mathf.Sqrt(delta)) / (2.0f * a);

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

        Vector3 ab = segment.P2 - segment.P1;
        Vector3 pa = segment.P1 - cylinder.P1;
        Vector3 pq = (cylinder.P2 - cylinder.P1).normalized;
        float r = cylinder.R;

        // On calcule delta
        float a = Vector3.Dot(ab, ab) - Mathf.Pow(Vector3.Dot(ab, pq), 2);
        float b = 2 * (Vector3.Dot(pa, ab) - Vector3.Dot(ab, pq) * Vector3.Dot(pa, pq));
        float c = Vector3.Dot(pa, pa) - Mathf.Pow(Vector3.Dot(pa, pq), 2) - Mathf.Pow(r, 2);

        // float a = Mathf.Pow(Vector3.Cross(pq, ab).magnitude, 2);
        // float b = 2.0f * Vector3.Dot(pa, ab) * Vector3.Dot(pq, pq) - 2.0f * Vector3.Dot(pa, pq) * Vector3.Dot(ab, pq);
        // float c = (Mathf.Pow(Vector3.Cross(pq, pa).magnitude, 2) - Mathf.Pow(r, 2)) * Vector3.Dot(pq, pq);
        float delta = b * b - 4.0f * a * c;
        

        if (delta < 0)
        {
            // Pas de point d'intersection
            return false;
        }

        // On calcule les valeurs t1 et t2
        float t1 = (-b - Mathf.Sqrt(delta)) / (2.0f * a);
        float t2 = (-b + Mathf.Sqrt(delta)) / (2.0f * a);

        // On retourne les valeurs d'intersection
        bool t1_01 = 0 <= t1 && t1 <= 1;
        bool t2_01 = 0 <= t2 && t2 <= 1;
        float t;

        if (t1_01 && t2_01)
        {
            t = Mathf.Min(t1, t2);
        } else if (t1_01)
        {
            t = t1;
        }
        else if (t2_01)
        {
            t = t2;
        } else {
            // S'ils ne sont pas sécants
            return false;
        }

        // S'ils sont sécants
        interP = segment.P1 + t * ab;

        // Calcul du projeté orthogonal du point d'intertion sur le vecteur directeur du cylindre
        float X = pq.x;
        float Y = pq.y;
        float Z = pq.z;
        float T = -((X * cylinder.P1.x - X * interP.x) + (Y * cylinder.P1.y - Y * interP.y) + (Z * cylinder.P1.z - Z * interP.z)) / (X * X + Y * Y + Z * Z);
        Vector3 projOrth = new Vector3(T * X + cylinder.P1.x, T * Y + cylinder.P1.y, T * Z + cylinder.P1.z);
        vectN = Vector3.Normalize(interP - projOrth);
        
        return true;
        
    }
}