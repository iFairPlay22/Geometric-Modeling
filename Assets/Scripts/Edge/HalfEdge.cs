using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HalfEdge
{
    public Vector3 sourceVertex;
    public HalfEdge previousHalfEdge;
    public HalfEdge nextHalfEdge;
    public HalfEdge twinHalfEdge;
    public Face face;
}