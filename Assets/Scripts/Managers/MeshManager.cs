using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.CustumForm
{
    ///<summary> Enumération des noms de formes possibles à subdiviser </summary>
    public enum FormEnum
    {
        Cube, StraightPrism
    }

    ///<summary> Gestionnaire de création et de modification de meshs </summary>
    public class MeshManager : MonoBehaviour
    {
        #region Attributes

        [SerializeField]
        ///<summary> Le matérial a injecter dans les game objects des formes crées </summary>
        private Material meshMaterial;

        ///<summary> Le type de forme à subdiviser </summary>
        [SerializeField]
        private FormEnum form;

        ///<summary> Le nombre de subdivisions consécutives à réaliser </summary>
        [SerializeField]
        private int subdisionNb = 2;

        ///<summary> Liste de Half Edge Mesh pour la subdivision</summary>
        private List<HalfEdgeMesh> halfEdgeMeshes = new List<HalfEdgeMesh>();

        ///<summary> Liste des positions des meshes subdivisés créés </summary>
        private List<Vector3> offsets = new List<Vector3>();

        ///<summary> Méthode de fonction utilisée pour le calcul de position de Mesh à partir de kx et kz </summary>
        private delegate Vector3 ComputePositionFromKxKz(float kx, float kz);

        #endregion Attributes

        private void Start()
        {
            // Mesh mesh;
            // mesh = CreateTriangleMesh();
            // mesh = CreateQuadMesh(new Vector3(4, 0, 2));
            // mesh = CreateCubeMesh();
            // mesh = CreateStraightPrismMesh();
            // mesh = CreateStripXZTriangles(new Vector3(4, 0, 2), 8);
            // mesh = CreatePlane(new Vector3(10, 0, 15), 3, 2);
            // mesh = CreatePlane(new Vector3(10, 0, 15), 3, 2);
            // mesh = CreateSphere(5, ShereTypes.DEMI); 
            // mesh = CreateRing(5, 2);
            // mesh = CreateStripXZQuads(new Vector3(10, 0, 15), 50);
            // CreateGameObjectMesh("Nom de forme", mesh, Vector3.zero);

            AddCatmullSubdivisions();
        }

        #region Subdivision

        ///<summary> Création des subdvisions consécutives </summary>
        private void AddCatmullSubdivisions()
        {
            Mesh baseMesh = form == FormEnum.Cube ? CreateCubeMesh() : CreateStraightPrismMesh();
            HalfEdgeMesh currentHalfEdgeMesh = null;

            for (int i = 0; i < Mathf.Abs(subdisionNb) + 1; i++)
            {
                if (i == 0)
                {
                    // Création de la forme initiale à la première itération
                    currentHalfEdgeMesh = new HalfEdgeMesh(baseMesh);
                }
                else
                {
                    // Subdivision successive des mesh pour les autres itérations
                    currentHalfEdgeMesh = CatmullClark.SubdiviseMesh(currentHalfEdgeMesh);
                }

                Mesh currentMesh = currentHalfEdgeMesh.ToQuadsMesh();
                Vector3 offset = new Vector3(i * 2 + 1 - subdisionNb * 2 - 5, 0, 0);
                CreateGameObjectMesh("CatmullMesh " + i, currentMesh, offset);
                
                offsets.Add(offset);
                halfEdgeMeshes.Add(currentHalfEdgeMesh);

                Debug.Log((i + 1) + " => " + currentHalfEdgeMesh.GetInfos());
                // Debug.Log(MeshDisplayInfo.ExportMeshCSV(currentMesh));

            }
        }

        ///<summary> Création d'un objet correspondant à une étape de la subdivision </summary>
        ///<param name="title">Titre du game object</param>
        ///<param name="mesh">Mesh à appliquer au game object</param>
        ///<param name="position">Position du game object dans la scene</param>
        ///<returns>Le game object créé</returns>
        private GameObject CreateGameObjectMesh(string title, Mesh mesh, Vector3 position)
        {
            GameObject go = new GameObject(title);

            go.transform.position = position;

            go.AddComponent<MeshRenderer>();
            Material[] materials = { meshMaterial };
            Debug.Log(materials[0]);
            go.GetComponent<MeshRenderer>().materials = materials;

            go.AddComponent<MeshFilter>();
            go.GetComponent<MeshFilter>().sharedMesh = mesh;

            go.AddComponent<MeshDisplayInfo>();

            return go;
        }

        #region Affichage

        ///<summary> Affichage des vertices et edges des mesh subdivisés </summary>
        private void OnDrawGizmos()
        {
            if (offsets != null && halfEdgeMeshes != null)
            {
                for (int i = 0; i < halfEdgeMeshes.Count; i++)
                {
                    DrawPoints(Color.green, halfEdgeMeshes[i].vertices, offsets[i]);
                    DrawLines(Color.black, halfEdgeMeshes[i].halfEdges, offsets[i]);
                }  
            }
        }

        ///<summary> Affichage des vertices des mesh subdivisés </summary>
        private void DrawPoints(Color color, List<Vector3> vertices, Vector3 position)
        {
            Gizmos.color = color;
            foreach (Vector3 pt in vertices)
                Gizmos.DrawSphere(pt + position, 0.025f);
        }

        ///<summary> Affichage des edges des mesh subdivisés </summary>
        private void DrawLines(Color color, List<HalfEdge> halfEdges, Vector3 position)
        {
            Gizmos.color = color;
            foreach (HalfEdge halfEdge in halfEdges)
                Gizmos.DrawLine(halfEdge.sourceVertex + position, halfEdge.nextHalfEdge.sourceVertex + position);
        }

        #endregion Affichage

        #endregion Subdivision

        #region Formes de base

        /// <summary> Crée un triangle à partir de la topologie de triangles </summary>
        private Mesh CreateTriangleMesh()
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

        /// <summary> Crée un carré à partir de la topologie de triangles </summary>
        private Mesh CreateQuadMesh(Vector3 size)
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

        /// <summary> Crée un cube à partir de la topologie de carrés </summary>
        private Mesh CreateCubeMesh()
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Cube";
            int i;

            Vector3[] vertices = new Vector3[8];
            i = 0;
            vertices[i++] = new Vector3(0, 0, 0);
            vertices[i++] = new Vector3(0, 0, 1);
            vertices[i++] = new Vector3(1, 0, 1);
            vertices[i++] = new Vector3(1, 0, 0);
            vertices[i++] = new Vector3(0, 1, 0);
            vertices[i++] = new Vector3(0, 1, 1);
            vertices[i++] = new Vector3(1, 1, 1);
            vertices[i++] = new Vector3(1, 1, 0);

            int[] quads = new int[6 * 4];
            i = 0;
            quads[i++] = 3;
            quads[i++] = 2;
            quads[i++] = 1;
            quads[i++] = 0;
            quads[i++] = 7;
            quads[i++] = 3;
            quads[i++] = 0;
            quads[i++] = 4;
            quads[i++] = 4;
            quads[i++] = 5;
            quads[i++] = 6;
            quads[i++] = 7;
            quads[i++] = 5;
            quads[i++] = 1;
            quads[i++] = 2;
            quads[i++] = 6;
            quads[i++] = 7;
            quads[i++] = 6;
            quads[i++] = 2;
            quads[i++] = 3;
            quads[i++] = 0;
            quads[i++] = 1;
            quads[i++] = 5;
            quads[i++] = 4;

            newMesh.vertices = vertices;
            newMesh.SetIndices(quads, MeshTopology.Quads, 0);
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();

            return newMesh;
        }

        /// <summary> Crée un prisme droit à partir de la topologie de carrés </summary>
        private Mesh CreateStraightPrismMesh()
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "Straight Prism";
            int i;

            Vector3[] vertices = new Vector3[8];
            i = 0;
            vertices[i++] = new Vector3(0, 0, -0.5f);
            vertices[i++] = new Vector3(0, 0, +1.5f);
            vertices[i++] = new Vector3(1, 0, +1.5f);
            vertices[i++] = new Vector3(1, 0, -0.5f);
            vertices[i++] = new Vector3(0, 1, 0);
            vertices[i++] = new Vector3(0, 1, 1);
            vertices[i++] = new Vector3(1, 1, 1);
            vertices[i++] = new Vector3(1, 1, 0);

            int[] quads = new int[6 * 4];
            i = 0;
            quads[i++] = 3;
            quads[i++] = 2;
            quads[i++] = 1;
            quads[i++] = 0;
            quads[i++] = 7;
            quads[i++] = 3;
            quads[i++] = 0;
            quads[i++] = 4;
            quads[i++] = 4;
            quads[i++] = 5;
            quads[i++] = 6;
            quads[i++] = 7;
            quads[i++] = 5;
            quads[i++] = 1;
            quads[i++] = 2;
            quads[i++] = 6;
            quads[i++] = 7;
            quads[i++] = 6;
            quads[i++] = 2;
            quads[i++] = 3;
            quads[i++] = 0;
            quads[i++] = 1;
            quads[i++] = 5;
            quads[i++] = 4;

            newMesh.vertices = vertices;
            newMesh.SetIndices(quads, MeshTopology.Quads, 0);
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();

            return newMesh;
        }

        #endregion Formes de base

        #region Bandes de triangles

        /// <summary> Création d'une bande 2D de triangles à partir de la topologie de triangles </summary>
        private Mesh CreateStripXZTriangles(Vector3 size, int nSegments)
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

        /// <summary> Création d'une bande 3D de triangles à partir de la topologie de triangles </summary>
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

        /// <summary> Création d'une bande 3D de triangles à partir de la topologie de triangles et d'une fonction de gestion de la structure du Mesh </summary>
        private Mesh CreateNormalizedPlane(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
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

        #endregion Bandes de triangles

        #region Formes à partir de triangles

        /// <summary> Types de formes possibles à dessiner vis à vis d'une sphère </summary>
        private enum ShereTypes { QUARTER, DEMI, FULL };

        /// <summary> On associe le type de forme de la sphère à un coefficient pour le calcul des coordonnées des points </summary>
        private Dictionary<ShereTypes, float> SphereTypesDict = new Dictionary<ShereTypes, float>(){
            {ShereTypes.QUARTER, 0.5f},
            {ShereTypes.DEMI, 1.0f},
            {ShereTypes.FULL, 2.0f}
        };

        /// <summary> Création d'une sphère à partir de la topologie de triangles </summary>
        private Mesh CreateSphere(int radius, ShereTypes sphereType, int nSegmentsX = 50, int nSegmentsZ = 50)
        {
            return CreateNormalizedPlane(nSegmentsX, nSegmentsZ, (kX, kZ) =>
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

        /// <summary> Création d'un anneau à partir de la topologie de triangles </summary>
        private Mesh CreateRing(int radius, int height, int nSegmentsX = 50, int nSegmentsZ = 50)
        {
            return CreateNormalizedPlane(nSegmentsX, nSegmentsZ, (kX, kZ) =>
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

        #endregion Formes à partir de triangles

        #region Bandes de carrés

        /// <summary> Création d'un bande 2D de carrés à partir de la topologie de carrés </summary>
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

        /// <summary> Création d'un bande 3D de carrés à partir de la topologie de carrés et d'une fonction de gestion de la structure du Mesh </summary>
        private Mesh CreateNormalizedPlaneQuads(int nSegmentsX, int nSegmentsZ, ComputePositionFromKxKz computePosition)
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

        #endregion Bandes de carrés
    }
}