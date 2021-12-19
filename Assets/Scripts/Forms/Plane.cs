using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Représentation mathématique d'un plan </summary>
public class Plane
{
    #region Attributes

    private Vector3 n;
    ///<summary> Vecteur normal unitaire au plan </summary>
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

    ///<summary> Construction d'un Plane </summary>
    ///<param name="n">Vecteur normal au plan</param>
    ///<param name="d">Distance signée</param>
    public Plane(Vector3 n, float d)
    {
        this.n = Vector3.Normalize(n);
        this.d = d;
    }

    ///<summary> Construction d'un Plane </summary>
    ///<param name="n">Vecteur normal au plan</param>
    ///<param name="p">Vecteur directeur du plan</param>
    public Plane(Vector3 n, Vector3 p)
    {
        this.n = Vector3.Normalize(n);
        this.d = Vector3.Dot(p, n);
    }

    ///<summary> Construction d'un Plane </summary>
    ///<param name="p1">Un point du plan différent de p2 et p3</param>
    ///<param name="p2">Un point du plan différent de p1 et p3</param>
    ///<param name="p3">Un point du plan différent de p1 et p2</param>
    public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.n = Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p1));
        this.d = Vector3.Dot(p1, n);
    }

    #endregion Constructors

    #region Calcul de distance

    ///<summary> Retourne la distance entre un plan et un point </summary>
    ///<param name="point">Le point à comparer</param>
    ///<returns>La distance en float</returns>
    public float GetDistance(Vector3 point)
    {
        // On récupère un point du plan
        Vector3 pointM = new Vector3(0, 0, -D / N.z);

        // On calcule la distance
        return Vector3.Dot(N, point - pointM);
    }

    #endregion Calcul de distance
}