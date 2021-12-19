using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Repr�sentation math�matique d'une sph�re </summary>
public class Sphere
{
    #region Attributes
    private Vector3 p;
    ///<summary> Centre de la sph�re </summary>
    public Vector3 P
    {
        get { return p; }
        private set { p = value; }
    }

    private float r;
    ///<summary> Rayon de la sph�re </summary>
    public float R
    {
        get { return r; }
        private set { r = value; }
    }

    #endregion Attributes

    ///<summary> Construction d'une Sphere </summary>
    ///<param name="p">Centre de la sph�re</param>
    ///<param name="r">Rayon de la sph�re</param>
    public Sphere(Vector3 p, float r)
    {
        this.p = p;
        this.r = Mathf.Abs(r);
    }
}