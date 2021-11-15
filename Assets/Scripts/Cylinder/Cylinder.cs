using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder
{
    private Vector3 p1;

    public Vector3 P1
    {
        get { return p1; }
        private set { p1 = value; }
    }

    private Vector3 p2;

    public Vector3 P2
    {
        get { return p2; }
        private set { p2 = value; }
    }

    private float r;

    public float R
    {
        get { return r; }
        set { r = value; }
    }

    public Cylinder(Vector3 p1, Vector3 p2, float r)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.r = Mathf.Abs(r);
    }
}