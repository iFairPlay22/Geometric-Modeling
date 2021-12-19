using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Line : Segment
{
    ///<summary> Construction d'une Line</summary>
    ///<param name="p1">Un point de la droite différent de p1</param>
    ///<param name="p2">Un point de la droite différent de p2</param>
    public Line(Vector3 p1, Vector3 p2) : base(p1, p2) {}

    #region Calcul de distance

    ///<summary> Retourne la distance entre une droite et un point </summary>
    ///<param name="point">Le point à comparer</param>
    ///<returns>La distance en float</returns>
    public float GetDistance(Vector3 point)
    {
        return Vector3.Magnitude(Vector3.Cross(P1 - point, P2 - P1) / Vector3.Magnitude(P2 - P1));
    }

    #endregion Calcul de distance
}