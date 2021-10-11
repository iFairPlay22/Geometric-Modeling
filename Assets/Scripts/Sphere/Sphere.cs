using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere
{
    private Vector3 p;

    public Vector3 P
    {
        get { return p; }
        private set { p = value; }
    }

    private float r;

    public float R
    {
        get { return r; }
        private set { r = value; }
    }

    public Sphere(Vector3 p, float r)
    {
        this.p = p;
        this.r = Mathf.Abs(r);
    }
}