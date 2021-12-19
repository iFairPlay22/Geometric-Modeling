using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Représentation mathématique d'un segment </summary>
public class Segment
{
    #region Attributes
    private Vector3 p1;
    ///<summary> Point de départ du segment </summary>
    public Vector3 P1
    {
        get { return p1; }
        set { p1 = value; }
    }

    private Vector3 p2;
    ///<summary> Point d'arrivée du segment </summary>
    public Vector3 P2
    {
        get { return p2; }
        set { p2 = value; }
    }

    #endregion Attributes

    ///<summary> Construction d'un Segment </summary>
    ///<param name="p1">Point de départ du segment</param>
    ///<param name="p2">Point d'arrivée du segment</param>
    public Segment(Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }
}