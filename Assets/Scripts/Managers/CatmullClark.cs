using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///<summary> Classe statique pour la gestion de subdivision de Mesh avec Catmull-Clark </summary>
class CatmullClark
{
    ///<summary> Subdivision d'HEMesh </summary>
    public static HalfEdgeMesh SubdiviseMesh(HalfEdgeMesh halfEdgeMesh)
    {
        HalfEdgeMesh newHalfEdgeMesh;

        // Création de Face Points
        Dictionary<Face, Vector3> facePoints = GetFacePoints(halfEdgeMesh);

        // Création de Edge Points
        Dictionary<HalfEdge, Vector3> edgePoints = GetEdgePoints(halfEdgeMesh, facePoints);

        // Update des Vertices
        Dictionary<Vector3, Vector3> vertexPoints = GetVertexPoints(halfEdgeMesh, facePoints);

        // Split des Edges
        newHalfEdgeMesh = GetMeshWithNewEdges(halfEdgeMesh, vertexPoints, edgePoints, facePoints);

        // Split des Faces
        newHalfEdgeMesh = GetMeshWithNewFaces(newHalfEdgeMesh, facePoints);

        return newHalfEdgeMesh;
    }

    #region SubdivisionSteps

    ///<summary> Création des Face Points et récupération de tous les isobarycentres </summary>
    public static Dictionary<Face, Vector3> GetFacePoints(HalfEdgeMesh halfEdgeMesh)
    {
        List<Face> faces = halfEdgeMesh.faces;
        Dictionary<Face, Vector3> d = new Dictionary<Face, Vector3>();

        // Pour toutes mes faces
        for (int i = 0; i < faces.Count; i++)
        {
            Face face = faces[i];
            HalfEdge halfEdge = face.halfEdge;

            // On récupère la somme des points de la face
            Vector3 pointsSum = new Vector3(0, 0, 0);
            for (int j = 0; j < 4; j++)
            {
                Vector3 point = halfEdge.sourceVertex;
                pointsSum += point;
                halfEdge = halfEdge.nextHalfEdge;
            }

            // On calcule le barycentre de la face
            Vector3 isobarycenter = pointsSum / 4;
            d[face] = isobarycenter;
        }

        return d;
    }

    ///<summary> Création de Edge Points et récupération des moyennes des vertices et des isobarycentres </summary>
    public static Dictionary<HalfEdge, Vector3> GetEdgePoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints)
    {
        List<HalfEdge> edges = halfEdgeMesh.halfEdges;
        Dictionary<HalfEdge, Vector3> d = new Dictionary<HalfEdge, Vector3>();

        // Pour tous mes segments
        for (int i = 0; i < edges.Count; i++)
        {
            // On récupère les faces et les extrémités de segments
            HalfEdge edge1 = edges[i];
            Vector3 vertex1 = edge1.sourceVertex;

            // Pas de twinHalfEdge
            if (edge1.twinHalfEdge == null)
            {
                HalfEdge edge2 = edge1.nextHalfEdge;
                Vector3 vertex2 = edge2.sourceVertex;
                d[edge1] = (vertex1 + vertex2) / 2.0f;
            }
            else
            {
                HalfEdge edge2 = edge1.twinHalfEdge;
                Vector3 vertex2 = edge2.sourceVertex;

                Face face1 = edge1.face;
                Vector3 face1Barycenter = facePoints[face1];

                Face face2 = edge2.face;
                Vector3 face2Barycenter = facePoints[face2];


                d[edge1] = (vertex1 + vertex2 + face1Barycenter + face2Barycenter) / 4.0f;
            }
        }

        return d;
    }

    ///<summary> Mise à jour des vertices </summary>
    public static Dictionary<Vector3, Vector3> GetVertexPoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints)
    {
        List<HalfEdge> edges = halfEdgeMesh.halfEdges;
        Dictionary<Vector3, List<HalfEdge>> edgesByOrigin = new Dictionary<Vector3, List<HalfEdge>>();

        // On associe chaque vertices à la liste des edges dont il est l'origine
        for (int i = 0; i < edges.Count; i++)
        {
            HalfEdge edge = edges[i];
            Vector3 point = edge.sourceVertex;

            if (!edgesByOrigin.ContainsKey(point))
               edgesByOrigin[point] = new List<HalfEdge>();
            edgesByOrigin[point].Add(edge);
        }

        // Pour tous les points
        Dictionary<Vector3, Vector3> d = new Dictionary<Vector3, Vector3>();

        for (int i = 0; i < halfEdgeMesh.vertices.Count; i++)
        {
            Vector3 vertice = halfEdgeMesh.vertices[i];
            List<HalfEdge> edgesOrigin = edgesByOrigin[vertice];

            // Calcul de N, Q, R
            float N = edgesOrigin.Count;
            Vector3 Q = Vector3.zero;
            Vector3 R = Vector3.zero;
            for (int j = 0; j < N; j++)
            {
                HalfEdge edgeOrigin = edgesOrigin[j];
                HalfEdge edgeOrigin2 = edgeOrigin.nextHalfEdge;
                R += (edgeOrigin.sourceVertex + edgeOrigin2.sourceVertex) / 2.0f;

                Face faceOrigin = edgeOrigin.face;
                Q += facePoints[faceOrigin];
            }
            Q /= N;
            R /= N;

            d[vertice] = (Q / N) + ((2.0f * R) / N) + ((N - 3.0f) / N) * vertice;
        }

        return d;
    }

    ///<summary> Division des Edges </summary>
    public static HalfEdgeMesh GetMeshWithNewEdges(HalfEdgeMesh halfEdgeMesh, Dictionary<Vector3, Vector3> vertexPoints, Dictionary<HalfEdge, Vector3> edgePoints, Dictionary<Face, Vector3> facePoints)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<HalfEdge> newEdges = new List<HalfEdge>();
        List<Face> newFaces = new List<Face>();

        // On associe chaque couple de vertices à un edge qu'il délimite
        PointsCouple p;
        Dictionary<PointsCouple, HalfEdge> verticesToHalfEdge = new Dictionary<PointsCouple, HalfEdge>();

        for (int i = 0; i < halfEdgeMesh.faces.Count; i++)
        {
            // Récupération des objets
            Face currentF = halfEdgeMesh.faces[i];
            HalfEdge currentE1 = currentF.halfEdge;
            HalfEdge currentE2 = currentE1.nextHalfEdge;
            HalfEdge currentE3 = currentE2.nextHalfEdge;
            HalfEdge currentE4 = currentE3.nextHalfEdge;

            // Création des objets
            Face newF = new Face();
            HalfEdge newE1 = new HalfEdge();
            HalfEdge newE2 = new HalfEdge();
            HalfEdge newE3 = new HalfEdge();
            HalfEdge newE4 = new HalfEdge();
            HalfEdge newE1f = new HalfEdge();
            HalfEdge newE2f = new HalfEdge();
            HalfEdge newE3f = new HalfEdge();
            HalfEdge newE4f = new HalfEdge();

            newF.halfEdge = newE1;

            newE1.sourceVertex = vertexPoints[currentE1.sourceVertex];
            newE1.twinHalfEdge = null;
            newE1.previousHalfEdge = newE4f;
            newE1.nextHalfEdge = newE1f;
            newE1.face = newF;

            newE2.sourceVertex = vertexPoints[currentE2.sourceVertex];
            newE2.twinHalfEdge = null;
            newE2.previousHalfEdge = newE1f;
            newE2.nextHalfEdge = newE2f;
            newE2.face = newF;

            newE3.sourceVertex = vertexPoints[currentE3.sourceVertex];
            newE3.twinHalfEdge = null;
            newE3.previousHalfEdge = newE2f;
            newE3.nextHalfEdge = newE3f;
            newE3.face = newF;

            newE4.sourceVertex = vertexPoints[currentE4.sourceVertex];
            newE4.twinHalfEdge = null;
            newE4.previousHalfEdge = newE3f;
            newE4.nextHalfEdge = newE4f;
            newE4.face = newF;

            newE1f.sourceVertex = edgePoints[currentE1];
            newE1f.twinHalfEdge = null;
            newE1f.previousHalfEdge = newE1;
            newE1f.nextHalfEdge = newE2;
            newE1f.face = newF;

            newE2f.sourceVertex = edgePoints[currentE2];
            newE2f.twinHalfEdge = null;
            newE2f.previousHalfEdge = newE2;
            newE2f.nextHalfEdge = newE3;
            newE2f.face = newF;

            newE3f.sourceVertex = edgePoints[currentE3];
            newE3f.twinHalfEdge = null;
            newE3f.previousHalfEdge = newE3;
            newE3f.nextHalfEdge = newE4;
            newE3f.face = newF;

            newE4f.sourceVertex = edgePoints[currentE4];
            newE4f.twinHalfEdge = null;
            newE4f.previousHalfEdge = newE4;
            newE4f.nextHalfEdge = newE1;
            newE4.face = newF;

            // Gestion des twins
            p = new PointsCouple(newE1.nextHalfEdge.sourceVertex, newE1.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE1.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE1;
            }

            p = new PointsCouple(newE2.nextHalfEdge.sourceVertex, newE2.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE2.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE2;
            }

            p = new PointsCouple(newE3.nextHalfEdge.sourceVertex, newE3.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE3.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE3;
            }

            p = new PointsCouple(newE4.nextHalfEdge.sourceVertex, newE4.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE4.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE4;
            }

            p = new PointsCouple(newE1f.nextHalfEdge.sourceVertex, newE1f.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE1f.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE1f;
            }

            p = new PointsCouple(newE2f.nextHalfEdge.sourceVertex, newE2f.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE2f.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE2f;
            }

            p = new PointsCouple(newE3f.nextHalfEdge.sourceVertex, newE3f.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE3f.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE3f;
            }

            p = new PointsCouple(newE4f.nextHalfEdge.sourceVertex, newE4f.sourceVertex);
            if (verticesToHalfEdge.ContainsKey(p))
            {
                newE4f.twinHalfEdge = verticesToHalfEdge[p];
                verticesToHalfEdge[p].twinHalfEdge = newE4f;
            }

            // On ajoute les edges correspondant au couple de points
            verticesToHalfEdge[new PointsCouple(newE1.sourceVertex, newE1.nextHalfEdge.sourceVertex)] = newE1;
            verticesToHalfEdge[new PointsCouple(newE2.sourceVertex, newE2.nextHalfEdge.sourceVertex)] = newE2;
            verticesToHalfEdge[new PointsCouple(newE3.sourceVertex, newE3.nextHalfEdge.sourceVertex)] = newE3;
            verticesToHalfEdge[new PointsCouple(newE4.sourceVertex, newE4.nextHalfEdge.sourceVertex)] = newE4;
            verticesToHalfEdge[new PointsCouple(newE1f.sourceVertex, newE1f.nextHalfEdge.sourceVertex)] = newE1f;
            verticesToHalfEdge[new PointsCouple(newE2f.sourceVertex, newE2f.nextHalfEdge.sourceVertex)] = newE2f;
            verticesToHalfEdge[new PointsCouple(newE3f.sourceVertex, newE3f.nextHalfEdge.sourceVertex)] = newE3f;
            verticesToHalfEdge[new PointsCouple(newE4f.sourceVertex, newE4f.nextHalfEdge.sourceVertex)] = newE4f;

            facePoints[newF] = facePoints[currentF];

            // Ajout des objets
            newVertices.Add(newE1.sourceVertex);
            newVertices.Add(newE2.sourceVertex);
            newVertices.Add(newE3.sourceVertex);
            newVertices.Add(newE4.sourceVertex);
            newVertices.Add(newE1f.sourceVertex);
            newVertices.Add(newE2f.sourceVertex);
            newVertices.Add(newE3f.sourceVertex);
            newVertices.Add(newE4f.sourceVertex);
            newEdges.Add(newE1);
            newEdges.Add(newE2);
            newEdges.Add(newE3);
            newEdges.Add(newE4);
            newEdges.Add(newE1f);
            newEdges.Add(newE2f);
            newEdges.Add(newE3f);
            newEdges.Add(newE4f);
            newFaces.Add(newF);
        }

        return new HalfEdgeMesh(newVertices, newEdges, newFaces);
    }

    ///<summary> Division des Faces </summary>
    public static HalfEdgeMesh GetMeshWithNewFaces(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints)
    {
        List<HalfEdge> newEdges = halfEdgeMesh.halfEdges;
        List<Face> newFaces = new List<Face>();
        List<Vector3> newVertices = new List<Vector3>(halfEdgeMesh.vertices);

        for (int i = 0; i < halfEdgeMesh.faces.Count; i++)
        {
            // Récupération des éléments
            Face currentF = halfEdgeMesh.faces[i];

            HalfEdge currentE0 = currentF.halfEdge;
            HalfEdge currentE1 = currentE0.nextHalfEdge;
            HalfEdge currentE2 = currentE1.nextHalfEdge;
            HalfEdge currentE3 = currentE2.nextHalfEdge;
            HalfEdge currentE4 = currentE3.nextHalfEdge;
            HalfEdge currentE5 = currentE4.nextHalfEdge;
            HalfEdge currentE6 = currentE5.nextHalfEdge;
            HalfEdge currentE7 = currentE6.nextHalfEdge;

            // Création des éléments
            Face newF1 = new Face();
            Face newF2 = new Face();
            Face newF3 = new Face();
            Face newF4 = new Face();

            HalfEdge newE8 = new HalfEdge();
            HalfEdge newE8t = new HalfEdge();
            HalfEdge newE9 = new HalfEdge();
            HalfEdge newE9t = new HalfEdge();
            HalfEdge newE10 = new HalfEdge();
            HalfEdge newE10t = new HalfEdge();
            HalfEdge newE11 = new HalfEdge();
            HalfEdge newE11t = new HalfEdge();

            newF1.halfEdge = currentE0;
            newF2.halfEdge = currentE2;
            newF3.halfEdge = currentE4;
            newF4.halfEdge = currentE6;

            newE8.sourceVertex = currentE1.sourceVertex;
            newE8.previousHalfEdge = currentE0;
            newE8.nextHalfEdge = newE11t;
            newE8.face = newF1;
            newE8.twinHalfEdge = newE8t;
            newE8.previousHalfEdge.nextHalfEdge = newE8;
            newE8.nextHalfEdge.previousHalfEdge = newE8;

            newE9.sourceVertex = currentE3.sourceVertex;
            newE9.previousHalfEdge = currentE2;
            newE9.nextHalfEdge = newE8t;
            newE9.face = newF2;
            newE9.twinHalfEdge = newE9t;
            newE9.previousHalfEdge.nextHalfEdge = newE9;
            newE9.nextHalfEdge.previousHalfEdge = newE9;

            newE10.sourceVertex = currentE5.sourceVertex;
            newE10.previousHalfEdge = currentE4;
            newE10.nextHalfEdge = newE9t;
            newE10.face = newF3;
            newE10.twinHalfEdge = newE10t;
            newE10.previousHalfEdge.nextHalfEdge = newE10;
            newE10.nextHalfEdge.previousHalfEdge = newE10;

            newE11.sourceVertex = currentE7.sourceVertex;
            newE11.previousHalfEdge = currentE6;
            newE11.nextHalfEdge = newE10t;
            newE11.face = newF4;
            newE11.twinHalfEdge = newE11t;
            newE11.previousHalfEdge.nextHalfEdge = newE11;
            newE11.nextHalfEdge.previousHalfEdge = newE11;

            newE8t.sourceVertex = facePoints[currentE0.face];
            newE8t.previousHalfEdge = newE9;
            newE8t.nextHalfEdge = currentE1;
            newE8t.face = newF2;
            newE8t.twinHalfEdge = newE8;
            newE8t.previousHalfEdge.nextHalfEdge = newE8t;
            newE8t.nextHalfEdge.previousHalfEdge = newE8t;

            newE9t.sourceVertex = facePoints[currentE1.face];
            newE9t.previousHalfEdge = newE10;
            newE9t.nextHalfEdge = currentE3;
            newE9t.face = newF2;
            newE9t.twinHalfEdge = newE9;
            newE9t.previousHalfEdge.nextHalfEdge = newE9t;
            newE9t.nextHalfEdge.previousHalfEdge = newE9t;

            newE10t.sourceVertex = facePoints[currentE2.face];
            newE10t.previousHalfEdge = newE11;
            newE10t.nextHalfEdge = currentE5;
            newE10t.face = newF2;
            newE10t.twinHalfEdge = newE10;
            newE10t.previousHalfEdge.nextHalfEdge = newE10t;
            newE10t.nextHalfEdge.previousHalfEdge = newE10t;

            newE11t.sourceVertex = facePoints[currentE3.face];
            newE11t.previousHalfEdge = newE8;
            newE11t.nextHalfEdge = currentE7;
            newE11t.face = newF2;
            newE11t.twinHalfEdge = newE11;
            newE11t.previousHalfEdge.nextHalfEdge = newE11t;
            newE11t.nextHalfEdge.previousHalfEdge = newE11t;

            currentE0.face = newF1;
            currentE1.face = newF2;
            currentE2.face = newF2;
            currentE3.face = newF3;
            currentE4.face = newF4;
            currentE5.face = newF4;
            currentE6.face = newF4;
            currentE7.face = newF1;

            newVertices.Add(newE8t.sourceVertex);
            newVertices.Add(newE9t.sourceVertex);
            newVertices.Add(newE10t.sourceVertex);
            newVertices.Add(newE11t.sourceVertex);

            newEdges.Add(newE8);
            newEdges.Add(newE8t);
            newEdges.Add(newE9);
            newEdges.Add(newE9t);
            newEdges.Add(newE10);
            newEdges.Add(newE10t);
            newEdges.Add(newE11);
            newEdges.Add(newE11t);

            newFaces.Add(newF1);
            newFaces.Add(newF2);
            newFaces.Add(newF3);
            newFaces.Add(newF4);
        }

        return new HalfEdgeMesh(newVertices, newEdges, newFaces);
    }

    #endregion SubdivisionSteps
}

///<summary> Couple générique de deux points </summary>
class PointsCouple
{
    private readonly List<Vector3> points = new List<Vector3> ();

    public PointsCouple(Vector3 a, Vector3 b) {
        points.Add(a);
        points.Add(b);
    }

    public override int GetHashCode()
    {
        return points[0].GetHashCode() + 2 * points[1].GetHashCode();
    }
    public override bool Equals(object b)
    {
        return Equals(b as PointsCouple);
    }
    public bool Equals(PointsCouple b)
    {
        return points[0].Equals(b.points[0]) && points[1].Equals(b.points[1]);
    }
}