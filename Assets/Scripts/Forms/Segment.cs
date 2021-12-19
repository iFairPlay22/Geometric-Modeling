using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Repr�sentation math�matique d'un segment </summary>
public class Segment
{
    #region Attributes
    private Vector3 p1;
    ///<summary> Point de d�part du segment </summary>
    public Vector3 P1
    {
        get { return p1; }
        set { p1 = value; }
    }

    private Vector3 p2;
    ///<summary> Point d'arriv�e du segment </summary>
    public Vector3 P2
    {
        get { return p2; }
        set { p2 = value; }
    }

    #endregion Attributes

    ///<summary> Construction d'un Segment </summary>
    ///<param name="p1">Point de d�part du segment</param>
    ///<param name="p2">Point d'arriv�e du segment</param>
    public Segment(Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }
}