using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

///<summary> Représentation d'un "côté" d'un edge, correspondant à une seule face d'un mesh </summary>
public class HalfEdge
{
    ///<summary> Vertex d'origine de l'HE </summary>
    public Vector3 sourceVertex;

    ///<summary> HalfEdge précédente de l'HE courant </summary>
    public HalfEdge previousHalfEdge;

    ///<summary> HalfEdge suivante de l'HE courant </summary>
    public HalfEdge nextHalfEdge;

    ///<summary> HalfEdge opposée de l'HE courant </summary>
    public HalfEdge twinHalfEdge;

    ///<summary> Face correspondante de l'HE </summary>
    public Face face;
}