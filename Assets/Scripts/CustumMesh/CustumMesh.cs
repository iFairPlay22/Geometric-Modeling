﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CustumForm
{
    [RequireComponent(typeof(MeshFilter))]
    public class CustumMesh : MonoBehaviour
    {
        private MeshFilter m_Mf;

        private delegate Vector3 ComputePositionFromKxKz(float kx, float kz);

        private void Awake()
        {
            m_Mf = GetComponent<MeshFilter>();

            // m_Mf.sharedMesh = CreateTriangle();
            // m_Mf.sharedMesh = CreateQuad(new Vector3(4, 0, 2));
            // m_Mf.sharedMesh = CreateStrip(new Vector3(4, 0, 2), 8);
            // m_Mf.sharedMesh = CreatePlane(new Vector3(10, 0, 15), 3, 2);

            m_Mf.sharedMesh = WrapNormalizePlaneQuads(3, 3, (kx, kz) => new Vector3(kx, 0, kz));
            m_Mf.sharedMesh = new HalfEdgeMesh(m_Mf.sharedMesh).toVertexFace();

            Debug.Log(MeshDisplayInfo.ExportMeshCSV(m_Mf.sharedMesh));
        }

        #region Formes classiques

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
            newMesh.RecalculateNormals();

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
            newMesh.RecalculateNormals();

            return newMesh;
        }

        #endregion Formes classiques

        #region Triangles Strip

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
            newMesh.RecalculateNormals();

            return newMesh;
        }

        private Mesh CreatePlane(Vector3 size, int nSegmentsX, int nSegmentsZ)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Plane";

            Vector3 halfSize = size * .5f;

            Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
            int index = 0;
            for (int i = 0; i < nSegmentsX + 1; i++)
            {
                float kx = (float)i / nSegmentsX;
                float lerpx = Mathf.Lerp(-halfSize.x, halfSize.x, kx);
                for (int j = 0; j < nSegmentsZ + 1; j++)
                {
                    float kz = (float)j / nSegmentsZ;
                    float lerpz = Mathf.Lerp(-halfSize.z, halfSize.z, kz);
                    vertices[index++] = new Vector3(lerpx, 0, lerpz);
                }
            }
            newMesh.vertices = vertices;

            int[] triangles = new int[2 * nSegmentsX * nSegmentsZ * 3];
            index = 0;
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
            newMesh.RecalculateNormals();

            return newMesh;
        }

        private Mesh CreateNormalizePlane(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Normalize plane";

            Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
            int index = 0;
            for (int i = 0; i < nSegmentsX + 1; i++)
            {
                float kx = (float)i / nSegmentsX;
                for (int j = 0; j < nSegmentsZ + 1; j++)
                {
                    float kz = (float)j / nSegmentsZ;
                    vertices[index++] = computePosition(kx, kz);
                }
            }
            newMesh.vertices = vertices;

            int[] triangles = new int[2 * nSegmentsX * nSegmentsZ * 3];
            index = 0;
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
            newMesh.RecalculateNormals();

            return newMesh;
        }

        private enum ShereTypes { QUARTER, DEMI, FULL };

        private Dictionary<ShereTypes, float> SphereTypesDict = new Dictionary<ShereTypes, float>(){
        {ShereTypes.QUARTER, 0.5f},
        {ShereTypes.DEMI, 1.0f},
        {ShereTypes.FULL, 2.0f}
    };

        private Mesh CreateSphere(int radius, ShereTypes sphereType, int nSegmentsX = 50, int nSegmentsZ = 50)
        {
            return CreateNormalizePlane(nSegmentsX, nSegmentsZ, (kX, kZ) =>
            {
                float rho = radius; // 2 * 1 + 0.25f * Mathf.Sin(kZ * Mathf.PI * 2 * 4);
                float theta = kX * SphereTypesDict[sphereType] * Mathf.PI;
                float phi = (1 - kZ) * Mathf.PI;
                // Conversion sphérique => cartésien
                return new Vector3(
                    rho * Mathf.Cos(theta) * Mathf.Sin(phi),
                    rho * Mathf.Cos(phi),
                    rho * Mathf.Sin(theta) * Mathf.Sin(phi)
                );
            }
            );
        }

        private Mesh CreateRing(int radius, int height, int nSegmentsX = 50, int nSegmentsZ = 50)
        {
            return CreateNormalizePlane(nSegmentsX, nSegmentsZ, (kX, kZ) =>
            {
                if (kX == 0)
                {
                    // Dessus du ring
                    return new Vector3();
                }
                else if (kX == 1)
                {
                    // Dessous du ring
                    return new Vector3();
                }
                else
                {
                    // Côtés du ring
                    float rho = radius;
                    float theta = (1 - kX) * 2 * Mathf.PI;
                    float y = kZ * height;
                    return new Vector3(rho * Mathf.Cos(theta), y, rho * Mathf.Sin(theta));
                }
            });
        }

        #endregion Triangles Strip

        #region Vertex face

        private Mesh CreateStripXZQuads(Vector3 size, int nSegments)
        {
            Vector3 halfSize = size * .5f;

            Mesh newMesh = new Mesh();
            newMesh.name = "stripQuads";

            Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
            int[] quads = new int[nSegments * 4];

            //Vertices
            for (int i = 0; i < nSegments + 1; i++)
            {
                float k = (float)i / nSegments;
                float y = .25f * Mathf.Sin(k * Mathf.PI * 2 * 3);
                vertices[i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, -halfSize.z);
                vertices[nSegments + 1 + i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, halfSize.z);
            }

            //Triangles
            int index = 0;
            for (int i = 0; i < nSegments; i++)
            {
                quads[index++] = i;
                quads[index++] = i + nSegments + 1;
                quads[index++] = i + nSegments + 2;
                quads[index++] = i + 1;
            }

            newMesh.vertices = vertices;
            newMesh.SetIndices(quads, MeshTopology.Quads, 0);
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();
            return newMesh;
        }

        private Mesh WrapNormalizePlaneQuads(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "plane";
            Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
            int[] quads = new int[nSegmentsX * nSegmentsZ * 4];

            //Vertices
            int index = 0;
            for (int i = 0; i < nSegmentsX + 1; i++)
            {
                float kX = (float)i / nSegmentsX;
                for (int j = 0; j < nSegmentsZ + 1; j++)
                {
                    float kZ = (float)j / nSegmentsZ;
                    vertices[index++] = computePosition(kX, kZ);
                }
            }

            //Quads
            index = 0;
            //double boucle également
            for (int i = 0; i < nSegmentsX; i++)
            {
                for (int j = 0; j < nSegmentsZ; j++)
                {
                    quads[index++] = i * (nSegmentsZ + 1) + j;
                    quads[index++] = i * (nSegmentsZ + 1) + j + 1;
                    quads[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;
                    quads[index++] = (i + 1) * (nSegmentsZ + 1) + j;
                }
            }

            newMesh.vertices = vertices;
            newMesh.SetIndices(quads, MeshTopology.Quads, 0);
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();

            return newMesh;
        }

        /*
         * Mesh CreateRegularPolygonXZQuads(float radius, int nQuads)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "RegularPolygonQuads";

            Vector3[] vertices = new Vector3[??];
            int[] quads = new int[nQuads * 4];

            vertices[0] = Vector3.zero;

            //Vertices
            for (int i = 0; i < ???; i++)
            {
            }

            //Quads
            int index = 0;
            for (int i = 0; i < nQuads; i++)
            {
            quads[index++] =????;
            quads[index++] =????;
            quads[index++] =????;
            quads[index++] =????;
            }

            newMesh.vertices = vertices;
            newMesh.SetIndices(quads, MeshTopology.Quads, 0);
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();
            return newMesh;
        }*/

        #endregion Vertex face
    }
}