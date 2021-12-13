using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class CatmullClark
{
    private static HalfEdgeMesh subdiviseMesh(HalfEdgeMesh halfEdgeMesh)
    {
        // Création de Face Points
        Dictionary<Face, Vector3> facePoints = getFacePoints(halfEdgeMesh);

        // Création de Edge Points
        Dictionary<HalfEdge, Vector3> edgePoints = getEdgePoints(halfEdgeMesh, facePoints);

        // Update des Vertices
        Vector3[] vertexPoints = getVertexPoints(halfEdgeMesh, facePoints, edgePoints);

        // Split des Edges
        List<HalfEdge> newHalfEdges = getNewEdges(halfEdgeMesh, edgePoints);
        
        // Split des Faces
        List<Face> newFaces = getNewFaces(halfEdgeMesh, facePoints);

        List<Vector3> newVerticesList = new List<Vector3>();
        newVerticesList.AddRange(halfEdgeMesh.vertices);
        newVerticesList.AddRange(vertexPoints);
        Vector3[] newVertices  = newVerticesList.ToArray();

        return new HalfEdgeMesh(newVertices, newHalfEdges, newFaces); 
    }

    private static Dictionary<Face, Vector3> getFacePoints(HalfEdgeMesh halfEdgeMesh)
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

    private static Dictionary<HalfEdge, Vector3> getEdgePoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> allVerticesIsobarycenters)
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

    private static Vector3[] getVertexPoints(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints, Dictionary<HalfEdge, Vector3> edgePoints)
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

            vertices[i] = (Q / N) + ((2.0f * R) / N) + ((N - 3.0f) / N) * vertice;
        }

        return vertices;
    }

    private static List<HalfEdge> getNewEdges(HalfEdgeMesh halfEdgeMesh, Dictionary<HalfEdge, Vector3> edgePoints)
    {
        // On calcule les nouvelles positions des vertices
        List<HalfEdge> edges = halfEdgeMesh.edges;

        // On associe chaque vertices à la liste des edges dont il est l'origine
        for (int i = 0; i < edges.Count; i++)
        {
            HalfEdge edge1 = edges[i];
            HalfEdge edge2 = edge1.nextHalfEdge;

            Vector3 vertex1 = edge1.sourceVertex;
            Vector3 newVertex = edgePoints[edge1];

            HalfEdge newEdge = new HalfEdge();
            newEdge.sourceVertex = newVertex;
            newEdge.previousHalfEdge = edge1;
            newEdge.nextHalfEdge = edge2;
            newEdge.face = edge1.face;
            newEdge.twinHalfEdge = edge1.twinHalfEdge;

            edge1.nextHalfEdge = newEdge;
            edge2.previousHalfEdge = newEdge;

            edges.Add(newEdge);
        }

        return edges;
    }

    private static List<Face> getNewFaces(HalfEdgeMesh halfEdgeMesh, Dictionary<Face, Vector3> facePoints)
    {
        return new List<Face>();
    }
}