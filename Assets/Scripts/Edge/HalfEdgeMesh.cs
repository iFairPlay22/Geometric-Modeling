using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HalfEdgeMesh
{
    public readonly Vector3[] vertices;

    public readonly List<HalfEdge> edges;

    public readonly List<Face> faces;

    public HalfEdgeMesh(Vector3[] vertices, List<HalfEdge> edges, List<Face> faces)
    {
        this.vertices = vertices;
        this.edges = edges;
        this.faces = faces;
    }

    public HalfEdgeMesh(Mesh mesh)
    {
        // Vérifications
        if (!mesh) throw new Exception("Mesh is null!");

        int[] quads = mesh.GetIndices(0);

        this.vertices = mesh.vertices;
        this.edges = new List<HalfEdge>();
        this.faces = new List<Face>();
        Dictionary<Vector2, HalfEdge> map = new Dictionary<Vector2, HalfEdge>();

        int index = 0;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            int i0 = quads[index++];
            int i1 = quads[index++];
            int i2 = quads[index++];
            int i3 = quads[index++];

            Vector3 p0 = this.vertices[i0];
            Vector3 p1 = this.vertices[i1];
            Vector3 p2 = this.vertices[i2];
            Vector3 p3 = this.vertices[i3];

            HalfEdge he0 = new HalfEdge();
            HalfEdge he1 = new HalfEdge();
            HalfEdge he2 = new HalfEdge();
            HalfEdge he3 = new HalfEdge();

            Face f = new Face();
            f.halfEdge = he0;

            he0.sourceVertex = p0;
            he0.previousHalfEdge = he3;
            he0.nextHalfEdge = he1;
            Vector2 v10 = new Vector2(i1, i0);
            if (map.ContainsKey(v10))
            {
                he0.twinHalfEdge = map[v10];
                he0.twinHalfEdge.twinHalfEdge = he0;
            }
            he0.face = f;

            he1.sourceVertex = p1;
            he1.previousHalfEdge = he0;
            he1.nextHalfEdge = he2;
            Vector2 v21 = new Vector2(i2, i1);
            if (map.ContainsKey(v21))
            {
                he1.twinHalfEdge = map[v21];
                he1.twinHalfEdge.twinHalfEdge = he1;
            }
            he1.face = f;

            he2.sourceVertex = p2;
            he2.previousHalfEdge = he1;
            he2.nextHalfEdge = he3;
            Vector2 v32 = new Vector2(i3, i2);
            if (map.ContainsKey(v32))
            {
                he2.twinHalfEdge = map[v32];
                he2.twinHalfEdge.twinHalfEdge = he2;
            }
            he2.face = f;

            he3.sourceVertex = p3;
            he3.previousHalfEdge = he2;
            he3.nextHalfEdge = he0;
            Vector2 v03 = new Vector2(i0, i3);
            if (map.ContainsKey(v03))
            {
                he3.twinHalfEdge = map[v03];
                he3.twinHalfEdge.twinHalfEdge = he3;
            }
            he3.face = f;

            f.halfEdge = he0;

            this.edges.Add(he0);
            this.edges.Add(he1);
            this.edges.Add(he2);
            this.edges.Add(he3);

            map.Add(new Vector3(i0, i1), he0);
            map.Add(new Vector3(i1, i2), he1);
            map.Add(new Vector3(i2, i3), he2);
            map.Add(new Vector3(i3, i0), he3);

            this.faces.Add(f);
        }

        // Afficher le nombre de edge sans twinEdge
        //
        // int nb = 0;
        // for (int i = 0; i < edges.Count; i++)
        //    if (edges[i].twinHalfEdge == null)
        //        nb++;
        // Debug.Log("edges[i].twinHalfEdge == null " + nb + " / " + edges.Count);
    }

    public Mesh toVertexFace()
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "Vertex face";
        newMesh.vertices = this.vertices;

        int[] newQuads = new int[this.faces.Count * 4];
        int index = 0;

        for (int i = 0; i < this.faces.Count; i++)
        {
            HalfEdge halfEdge = this.faces[i].halfEdge;

            for (int j = 0; j < 4; j++)
            {
                Vector3 point = halfEdge.sourceVertex;
                newQuads[index++] = Array.IndexOf(this.vertices, point);
                halfEdge = halfEdge.nextHalfEdge;
            }
        }

        newMesh.SetIndices(newQuads, MeshTopology.Quads, 0);

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }

    public string GetInfos()
    {
        return "Vertices : " + vertices.Length + ", Edges: " + edges.Count + ", Faces: " + faces.Count;
    }
}