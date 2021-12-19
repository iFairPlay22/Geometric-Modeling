using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

///<summary> Mesh constitué de HalfEdges </summary>
public class HalfEdgeMesh
{
    #region Attributes
    ///<summary> Liste des vertices du mesh </summary>
    public readonly List<Vector3> vertices;

    ///<summary> Liste des HalfEdges du mesh </summary>
    public readonly List<HalfEdge> halfEdges;

    ///<summary> Liste des Faces du mesh </summary>
    public readonly List<Face> faces;

    #endregion Attributes

    #region Constructors

    ///<summary> Construction d'un HalfEdgeMesh </summary>
    ///<param name="vertices">Points du mesh</param>
    ///<param name="halfEdges">Segments du mesh</param>
    ///<param name="faces">Faces du mesh</param>
    public HalfEdgeMesh(List<Vector3> vertices, List<HalfEdge> halfEdges, List<Face> faces)
    {
        this.vertices = vertices;
        this.halfEdges = halfEdges;
        this.faces = faces;
        
    }

    ///<summary> Construction d'un HEMesh à partir d'un mesh standard de quads </summary>
    public HalfEdgeMesh(Mesh mesh)
    {
        // Vérification que le mesh ne soit pas null
        if (!mesh) throw new Exception("Mesh is null!");

        //Initialisation des attributs et des variables locales
        int[] quads = mesh.GetIndices(0);
        this.vertices = mesh.vertices.ToList();
        this.halfEdges = new List<HalfEdge>();
        this.faces = new List<Face>();
        Dictionary<Vector2, HalfEdge> map = new Dictionary<Vector2, HalfEdge>();

        //Création du mesh
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

            this.halfEdges.Add(he0);
            this.halfEdges.Add(he1);
            this.halfEdges.Add(he2);
            this.halfEdges.Add(he3);

            map.Add(new Vector3(i0, i1), he0);
            map.Add(new Vector3(i1, i2), he1);
            map.Add(new Vector3(i2, i3), he2);
            map.Add(new Vector3(i3, i0), he3);

            this.faces.Add(f);
        }
    }

    #endregion Constructors

    #region Functions

    ///<summary> Conversion d'un HEMesh en mesh standard de quads </summary>
    public Mesh ToQuadsMesh()
    {
        //Initialisation 
        Mesh newMesh = new Mesh();
        newMesh.name = "Vertex face";
        newMesh.vertices = this.vertices.ToArray();
        int[] newQuads = new int[this.faces.Count * 4];
        int index = 0;

        //Indexation
        for (int i = 0; i < this.faces.Count; i++)
        {
            HalfEdge halfEdge = this.faces[i].halfEdge;

            for (int j = 0; j < 4; j++)
            {
                Vector3 point = halfEdge.sourceVertex;
                newQuads[index++] = Array.IndexOf(newMesh.vertices, point);
                halfEdge = halfEdge.nextHalfEdge;
            }
        }

        //On set nos quads
        newMesh.SetIndices(newQuads, MeshTopology.Quads, 0);

        //Et on retourne le nouveau mesh
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    ///<summary> Retourne une chaîne de caractères qui liste le nombre de vertices, d'edges et de faces </summary>
    public string GetInfos()
    {
        return "Vertices : " + vertices.Count + ", Edges: " + halfEdges.Count + ", Faces: " + faces.Count;
    }

    #endregion  Function
}