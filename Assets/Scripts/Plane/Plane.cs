using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane
{
    // Vecteur normal au plan
    private Vector3 n;

    public Vector3 N
    {
        get { return n; }
        private set { n = value; }
    }

    // Distance signée
    private float d;

    public float D
    {
        get { return d; }
        set { d = value; }
    }

    public Plane(Vector3 n, float d)
    {
        this.n = Vector3.Normalize(n);
        this.d = d;
    }

    public Plane(Vector3 n, Vector3 p)
    {
        this.n = Vector3.Normalize(n);
        this.d = Vector3.Dot(p, n);
    }

    public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.n = Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p1));
        this.d = Vector3.Dot(p1, n);
    }
}