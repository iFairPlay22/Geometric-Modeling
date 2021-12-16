using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class CatmullClark
{
    public static HalfEdgeMesh subdiviseMesh(HalfEdgeMesh halfEdgeMesh)
    {
        // Création de Face Points
        Dictionary<Face, Vector3> facePoints = getFacePoints(halfEdgeMesh);

        // Création de Edge Points
        Dictionary<HalfEdge, Vector3> edgePoints = getEdgePoints(halfEdgeMesh, facePoints);

        // Update des Vertices
        Dictionary<Vector3, Vector3> vertexPoints = getVertexPoints(halfEdgeMesh, facePoints);

        // Split des Edges
        List<HalfEdge> newHalfEdges = getNewEdges(halfEdgeMesh, vertexPoints, edgePoints);

        // Split des Faces
        return getNewFaces(halfEdgeMesh, newHalfEdges, edgePoints, facePoints, vertexPoints);
    }

    public static Dictionary<Face, Vector3> getFacePoints(HalfEdgeMesh halfEdgeMesh)
    {

        // On récupère tous les isobarycentres des faces de mon mesh
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

    public static Dictionary<HalfEdge, Vector3> getEdgePoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> allVerticesIsobarycenters)
    {
        // On récupère les moyennes des vertices et des isobarycentres des faces de mon mesh
        List<HalfEdge> edges = halfEdgeMesh.edges;
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
                Vector3 face1Barycenter = allVerticesIsobarycenters[face1];

                Face face2 = edge2.face;
                Vector3 face2Barycenter = allVerticesIsobarycenters[face2];


                d[edge1] = (vertex1 + vertex2 + face1Barycenter + face2Barycenter) / 4.0f;
            }
        }

        return d;
    }

    public static Dictionary<Vector3, Vector3> getVertexPoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints)
    {
        // On calcule les nouvelles positions des vertices
        List<HalfEdge> edges = halfEdgeMesh.edges;
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
        Vector3[] vertices = halfEdgeMesh.vertices;
        Dictionary<Vector3, Vector3> d = new Dictionary<Vector3, Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertice = vertices[i];
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

    public static List<HalfEdge> getNewEdges(HalfEdgeMesh halfEdgeMesh, Dictionary<Vector3, Vector3> vertexPoints, Dictionary<HalfEdge, Vector3> edgePoints)
    {
        // On calcule les nouvelles positions des vertices
        List<HalfEdge> edges = halfEdgeMesh.edges;
        List<HalfEdge> newEdges = new List<HalfEdge>();

        // On associe chaque vertices à la liste des edges dont il est l'origine
        for (int i = 0; i < edges.Count; i++)
        {
            HalfEdge e = edges[i];

            // Update de la position de la vertice initiale du segment
            e.sourceVertex = vertexPoints[e.sourceVertex];

            HalfEdge eP1 = edges[i].nextHalfEdge;

            HalfEdge eN = new HalfEdge();
            eN.sourceVertex = edgePoints[e];
            eN.previousHalfEdge = e;
            eN.nextHalfEdge = eP1;
            eN.face = null;
            eN.twinHalfEdge = null;

            e.nextHalfEdge = eN;
            eP1.previousHalfEdge = eN;

            newEdges.Add(eN);
        }

        newEdges.AddRange(edges);
        return newEdges;
    }

    public static HalfEdgeMesh getNewFaces(HalfEdgeMesh halfEdgeMesh, List<HalfEdge> edges, Dictionary<HalfEdge, Vector3> edgePoints, Dictionary<Face, Vector3> facePoints, Dictionary<Vector3, Vector3> vertexPoints)
    {
        List<Face> faces = halfEdgeMesh.faces;
        List<Face> newFaces = new List<Face>();
        List<HalfEdge> newEdges = edges;

        for (int i = 0; i < faces.Count; i++)
        {
            Face face = faces[i];
            HalfEdge e0 = face.halfEdge;
            HalfEdge e1 = e0.nextHalfEdge;
            HalfEdge e2 = e1.nextHalfEdge;
            HalfEdge e3 = e2.nextHalfEdge;
            HalfEdge e4 = e3.nextHalfEdge;
            HalfEdge e5 = e4.nextHalfEdge;
            HalfEdge e6 = e5.nextHalfEdge;
            HalfEdge e7 = e6.nextHalfEdge;

            HalfEdge e8 = new HalfEdge();
            HalfEdge e8t = new HalfEdge();
            HalfEdge e9 = new HalfEdge();
            HalfEdge e9t = new HalfEdge();
            HalfEdge e10 = new HalfEdge();
            HalfEdge e10t = new HalfEdge();
            HalfEdge e11 = new HalfEdge();
            HalfEdge e11t = new HalfEdge();

            e8.sourceVertex = e1.sourceVertex;
            e8.previousHalfEdge = e0;
            e8.nextHalfEdge = e11t;

            e9.sourceVertex = e3.sourceVertex;
            e9.previousHalfEdge = e2;
            e9.nextHalfEdge = e8t;

            e10.sourceVertex = e5.sourceVertex;
            e10.previousHalfEdge = e4;
            e10.nextHalfEdge = e9t;

            e11.sourceVertex = e7.sourceVertex;
            e11.previousHalfEdge = e6;
            e11.nextHalfEdge = e10t;

            e8t.sourceVertex = facePoints[e0.face];
            e8t.previousHalfEdge = e11;
            e8t.nextHalfEdge = e0.twinHalfEdge;

            e9t.sourceVertex = facePoints[e0.face];
            e9t.previousHalfEdge = e8;
            e9t.nextHalfEdge = e2.twinHalfEdge;

            e10t.sourceVertex = facePoints[e0.face];
            e10t.previousHalfEdge = e9;
            e10t.nextHalfEdge = e4.twinHalfEdge;

            e11t.sourceVertex = facePoints[e0.face];
            e11t.previousHalfEdge = e10;
            e11t.nextHalfEdge = e6.twinHalfEdge;

            Face f0 = new Face();
            f0.halfEdge = e0;
            newFaces.Add(f0);

            Face f1 = new Face();
            f1.halfEdge = e1;
            newFaces.Add(f1);

            Face f2 = new Face();
            f2.halfEdge = e3;
            newFaces.Add(f2);

            Face f3 = new Face();
            f3.halfEdge = e5;
            newFaces.Add(f3);

            e7.face = f0;
            e0.face = f0;
            e8.face = f0;
            e11t.face = f0;

            e1.face = f1;
            e2.face = f1;
            e9.face = f1;
            e8t.face = f1;

            e3.face = f2;
            e4.face = f2;
            e10.face = f2;
            e9t.face = f2;

            e5.face = f3;
            e6.face = f3;
            e11.face = f3;
            e10t.face = f3;

            newEdges.Add(e0);
            newEdges.Add(e1);
            newEdges.Add(e2);
            newEdges.Add(e3);
            newEdges.Add(e4);
            newEdges.Add(e5);
            newEdges.Add(e6);
            newEdges.Add(e7);
            newEdges.Add(e8);
            newEdges.Add(e9);
            newEdges.Add(e10);
            newEdges.Add(e11);
            newEdges.Add(e8t);
            newEdges.Add(e9t);
            newEdges.Add(e10t);
            newEdges.Add(e11t);
        }

        // Ajout des vertices
        List<Vector3> newVertices = new List<Vector3>();
        newVertices.AddRange(new List<Vector3>(vertexPoints.Values));
        newVertices.AddRange(new List<Vector3>(facePoints.Values));
        newVertices.AddRange(new List<Vector3>(edgePoints.Values));

        return new HalfEdgeMesh(newVertices.ToArray(), newEdges, newFaces);
    }
}