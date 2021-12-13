using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    private Vector3 p1;

    public Vector3 P1
    {
        get { return p1; }
        set { p1 = value; }
    }

    private Vector3 p2;

    public Vector3 P2
    {
        get { return p2; }
        set { p2 = value; }
    }

    public Segment(Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }
}