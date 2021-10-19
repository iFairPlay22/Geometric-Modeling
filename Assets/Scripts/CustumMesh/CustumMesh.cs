using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CustumForm
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        private MeshFilter m_Mf;

        private void Awake()
        {
            m_Mf = GetComponent<MeshFilter>();
            // m_Mf.sharedMesh = CreateTriangle();
            // m_Mf.sharedMesh = CreateQuad(new Vector3(4, 0, 2));
            // m_Mf.sharedMesh = CreateStrip(new Vector3(4, 0, 2), 8);
            // m_Mf.sharedMesh = CreatePlane(new Vector3(10, 0, 15), 3, 2);
        }

        private Mesh CreateTriangle()
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Triangle";

            Vector3[] vertices = new Vector3[3];
            vertices[0] = Vector3.right;
            vertices[1] = Vector3.up;
            vertices[2] = Vector3.forward;
            newMesh.vertices = vertices;

            int[] triangles = new int[3];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            newMesh.triangles = triangles;

            newMesh.RecalculateBounds();

            return newMesh;
        }

        private Mesh CreateQuad(Vector3 size)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Quad";

            Vector3 halfSize = size * .5f;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z);
            vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z);
            vertices[2] = new Vector3(halfSize.x, 0, halfSize.z);
            vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z);
            newMesh.vertices = vertices;

            int[] triangles = new int[2 * 3];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            newMesh.triangles = triangles;
            newMesh.RecalculateBounds();

            return newMesh;
        }

        private Mesh CreateStrip(Vector3 size, int nSegments)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Strip";

            Vector3 halfSize = size * .5f;

            Vector3[] vertices = new Vector3[2 * (nSegments + 1)];
            float offsetIncrement = size.x / nSegments;
            float x = -offsetIncrement * .5f;
            for (int i = 0; i < vertices.Length; i += 2)
            {
                vertices[i] = new Vector3(x, 0, halfSize.z);
                vertices[i + 1] = new Vector3(x, 0, -halfSize.z);
                x += offsetIncrement;
            }
            newMesh.vertices = vertices;

            int[] triangles = new int[2 * nSegments * 3];
            int index = 0;
            for (int i = 0; i < nSegments * 2; i += 2)
            {
                triangles[index++] = i + 1;
                triangles[index++] = i + 0;
                triangles[index++] = i + 2;
                triangles[index++] = i + 1;
                triangles[index++] = i + 2;
                triangles[index++] = i + 3;
            }
            newMesh.triangles = triangles;

            newMesh.RecalculateBounds();

            return newMesh;
        }

        private Mesh CreatePlane(Vector3 size, int nSegmentsX, int nSegmentsZ)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Plane";

            Vector3 halfSize = size * .5f;

            Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
            for (int i = 0; i < nSegmentsX + 1; i++)
            {
                float kx = (float)i / nSegmentsX;
                float lerpx = Mathf.Lerp(-halfSize.x, halfSize.x, kx);
                for (int j = 0; j < nSegmentsZ + 1; j++)
                {
                    float kz = (float)j / nSegmentsZ;
                    float lerpz = Mathf.Lerp(-halfSize.z, halfSize.z, kz);
                    vertices[i * (nSegmentsZ + 1) + j] = new Vector3(lerpx, 0, lerpz);
                }
            }
            newMesh.vertices = vertices;

            int[] triangles = new int[2 * nSegmentsX * nSegmentsZ * 3];
            int index = 0;
            for (int i = 0; i < nSegmentsX; i++)
            {
                for (int j = 0; j < nSegmentsZ; j++)
                {
                    int currentIndex = i * (nSegmentsZ + 1) + j;

                    triangles[index++] = currentIndex + 1;
                    triangles[index++] = currentIndex + 0;
                    triangles[index++] = currentIndex + nSegmentsZ + 1;

                    triangles[index++] = currentIndex + 1;
                    triangles[index++] = currentIndex + nSegmentsZ + 1;
                    triangles[index++] = currentIndex + nSegmentsZ + 1 + 1;
                }
            }
            newMesh.triangles = triangles;

            newMesh.RecalculateBounds();

            return newMesh;
        }
    }
}