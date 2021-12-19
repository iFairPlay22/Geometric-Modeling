using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///<summary> Gestionnaire d'affichage et d'export des composants du mesh </summary>
[RequireComponent(typeof(MeshFilter))]
public class MeshDisplayInfo : MonoBehaviour
{
    ///<summary> MeshFilter de l'objet </summary>
    private MeshFilter m_Mf;

    [Header("Edges")]
    ///<summary> Afficher ou non le debug des edges </summary>
    [SerializeField] private bool m_DisplayEdges;
    ///<summary> Nombre maximum d'edges </summary>
    [SerializeField] private int m_NMaxEdges;

    [Header("Normals")]
    ///<summary> Afficher ou non les normales </summary>
    [SerializeField] private bool m_DisplayNormals;
    ///<summary> Nombre maximum de normales à afficher </summary>
    [SerializeField] private int m_NMaxNormals;
    ///<summary> Facteur pour la taille des normales </summary>
    [SerializeField] private float m_NormalScaleFactor;

    [Header("Vertices")]
    ///<summary> Afficher ou non le debug des vertices </summary>
    [SerializeField] private bool m_DisplayVertices;
    ///<summary> Nombre maximum de vertices </summary>
    [SerializeField] private int m_NMaxVertices;

    [Header("Faces")]
    ///<summary> Afficher ou non le debug des faces </summary>
    [SerializeField] private bool m_DisplayFaces;
    ///<summary> Nombre maximum de faces </summary>
    [SerializeField] private int m_NMaxFaces;

    private void Awake()
    {
        //Récupération du MeshFilter
        m_Mf = GetComponent<MeshFilter>();
    }

    ///<summary> Affichage des informations du mesh </summary>
    private void OnDrawGizmos()
    {
        if (!(m_Mf && m_Mf.sharedMesh)) return;

        Vector3[] vertices = m_Mf.sharedMesh.vertices;

        //Debug des edges et des faces
        if (m_DisplayEdges || m_DisplayFaces)
        {
            GUIStyle myStyle = null;

            if (m_DisplayFaces)
            {
                myStyle = new GUIStyle();
                myStyle.fontSize = 16;
                myStyle.normal.textColor = Color.blue;
            }

            int[] quads = m_Mf.sharedMesh.GetIndices(0);

            int index = 0;
            for (int i = 0; i < Mathf.Min(quads.Length / 4, Mathf.Max(m_NMaxEdges, m_NMaxFaces)); i++)
            {
                int index1 = quads[index++];
                int index2 = quads[index++];
                int index3 = quads[index++];
                int index4 = quads[index++];

                Vector3 pt1 = transform.TransformPoint(vertices[index1]);
                Vector3 pt2 = transform.TransformPoint(vertices[index2]);
                Vector3 pt3 = transform.TransformPoint(vertices[index3]);
                Vector3 pt4 = transform.TransformPoint(vertices[index4]);

                if (m_DisplayEdges && i < m_NMaxEdges)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(pt1, pt2);
                    Gizmos.DrawLine(pt3, pt2);
                    Gizmos.DrawLine(pt1, pt4);
                    Gizmos.DrawLine(pt4, pt3);
                }
                if (m_DisplayFaces && i < m_NMaxFaces)
                {
                    string str = $"{i}:{index1},{index2},{index3},{index4}";
                    Vector3 faceCenter = (pt1 + pt2 + pt3 + pt4) * .25f;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(faceCenter, .01f);
                    Handles.Label(faceCenter, str, myStyle);
                }
            }
        }

        //Debug des normales
        if (m_DisplayNormals)
        {
            Vector3[] normals = m_Mf.sharedMesh.normals;

            Gizmos.color = Color.white;

            for (int i = 0; i < Mathf.Min(normals.Length, m_NMaxNormals); i++)
            {
                Vector3 pos = transform.TransformPoint(vertices[i]);
                Vector3 normal = transform.TransformDirection(normals[i]);

                Gizmos.DrawLine(pos, pos + normal * m_NormalScaleFactor);
            }
        }

        //Debug des vertices
        if (m_DisplayVertices)
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 16;
            myStyle.normal.textColor = Color.red;

            Gizmos.color = Color.red;

            for (int i = 0; i < Mathf.Min(vertices.Length, m_NMaxVertices); i++)
            {
                Vector3 pos = transform.TransformPoint(vertices[i]);
                Gizmos.DrawSphere(pos, .01f);
                Handles.Label(pos, i.ToString(), myStyle);
            }
        }
    }

    ///<summary> Export des informations du mesh au format CSV </summary>
    public static string ExportMeshCSV(Mesh mesh)
    {
        if (!mesh) return "";

        List<string> strings = new List<string>();
        strings.Add("VertexIndex	VertexPosX	VertexPosY	VertexPosZ	QuadIndex	QuadVertexIndex1	QuadVertexIndex2	QuadVertexIndex3	QuadVertexIndex4");

        Vector3[] vertices = mesh.vertices;
        int[] quads = mesh.GetIndices(0);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            strings.Add($"{i}\t{pos.x.ToString("N02")}\t{pos.y.ToString("N02")}\t{pos.z.ToString("N02")}\t");
        }

        int index = 0;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            string tmpStr = $"{i}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}";
            if (i + 1 < strings.Count) strings[i + 1] += tmpStr;
            else strings.Add("\t\t\t\t" + tmpStr);
        }

        return string.Join("\n", strings);
    }
}