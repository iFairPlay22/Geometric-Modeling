using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Représentation mathématique d'un plan </summary>
public class Plane
{
    #region Attributes

    private Vector3 n;
    ///<summary> Vecteur normal au plan </summary>
    public Vector3 N
    {
        get { return n; }
        private set { n = value; }
    }

    private float d;
    ///<summary> Distance signée </summary>
    public float D
    {
        get { return d; }
        private set { d = value; }
    }

    #endregion Attributes

    #region Constructors

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

    #endregion Constructors
}