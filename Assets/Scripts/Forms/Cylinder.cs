using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Représentation mathématique d'un cylindre </summary>
public class Cylinder
{
    #region Attributes
    private Vector3 p1;
    ///<summary> Point de départ du cylindre </summary>
    public Vector3 P1
    {
        get { return p1; }
        private set { p1 = value; }
    }

    private Vector3 p2;
    ///<summary> Point d'arrivée du cylindre</summary>
    public Vector3 P2
    {
        get { return p2; }
        private set { p2 = value; }
    }

    private float r;
    ///<summary> Rayon du cylindre </summary>
    public float R
    {
        get { return r; }
        private set { r = value; }
    }

    #endregion Attributes

    ///<summary> Construction d'un cylindre à partir de deux points et d'un rayon </summary>
    ///<param name="p1">Point de départ du cylindre</param>
    ///<param name="p2">Point d'arrivée du cylindre</param>
    ///<param name="r">Rayon du cylindre</param>
    public Cylinder(Vector3 p1, Vector3 p2, float r)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.r = Mathf.Abs(r);
    }
}