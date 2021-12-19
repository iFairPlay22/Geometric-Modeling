using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///<summary> Gestionnaire d'affichage des formes géométriques et de leurs intersections dans la scène </summary>
public class Geometry : MonoBehaviour
{
    #region Attributes

    #region Point attributes

    ///<summary> Représentation mathématique d'une droite </summary>
    private Vector3 point = new Vector3(38, 2, 0);

    ///<summary> Vecteur des coefficients de rotation du point </summary>
    private Vector3 pointRotation = new Vector3(1, 1, 0);

    #endregion Line attributes

    #region Line attributes

    ///<summary> Représentation mathématique d'une droite </summary>
    private Line line = new Line(new Vector3(37, 0, 0), new Vector3(40, 0, 0));

    #endregion Line attributes

    #region Segment attributes

    ///<summary> Vecteur des coefficients de rotation du segment </summary>
    private Vector3 segmentRotation = new Vector3(1, 1, 1);

    ///<summary> Représentation mathématique du segment </summary>
    private Segment segment = new Segment(new Vector3(0, 0, 0), new Vector3(35, 0, 0));

    #endregion Segment attributes

    #region Plane attributes

    ///<summary> GameObject du plan </summary>
    [SerializeField]
    private GameObject planeObject;

    ///<summary> Représentation mathématique du plan </summary>
    private Plane plane = new Plane(new Vector3(10, 5, 10), 5);

    #endregion Plane attributes

    #region Sphere attributes

    ///<summary> Représentation mathématique de la sphère </summary>
    private Sphere sphere = new Sphere(new Vector3(15, 0, 0), 1.5f);

    #endregion Sphere attributes

    #region Cylinder attributes

    ///<summary> GameObject du cylindre </summary>
    [SerializeField]
    private GameObject cylinderObject;

    // Cylindre mathématique
    private Cylinder cylinder = new Cylinder(new Vector3(30, -5, 0), new Vector3(30, 8, 0), 1.5f);

    #endregion Cylinder attributes

    #region GUI attributes

    /// <summary> Style de GUI utilisé par défaut </summary>
    private GUIStyle gUIStyle = new GUIStyle();

    #endregion GUI attributes

    #endregion Attributes

    #region Printing


    private void Update()
    {
        #region Point printing

        // On mets à jour les coefficients de rotation du point

        if (point.x < line.P1.x) pointRotation.x = 1;
        else if (line.P2.x < point.x) pointRotation.x = -1;

        if (point.y < 0) pointRotation.y = 1;
        else if (2 < point.y) pointRotation.y = -1;

        // On bouge le point
        Vector3 moveV = pointRotation / 2 * Time.deltaTime;
        point += moveV;

        #endregion Point printing

        #region Segment printing

        // On mets à jour les coefficients de rotation du segment
        if (segment.P1.x < -10) segmentRotation.x = 1;
        else if (10 < segment.P1.x) segmentRotation.x = -1;

        if (segment.P1.y < -5) segmentRotation.y = 1;
        else if (5 < segment.P1.y) segmentRotation.y = -1;
        
        if (segment.P1.z < -2) segmentRotation.z = 1;
        else if (2 < segment.P1.z) segmentRotation.z = -1;

        // On bouge le segment
        moveV = segmentRotation * Time.deltaTime;
        segment.P1 += moveV;
        segment.P2 += moveV;

        #endregion

        #region Plane printing

        // On affiche le plan
        if (planeObject != null)
        {
            planeObject.transform.LookAt(plane.N);
            planeObject.transform.position = plane.N * plane.D;
        }

        #endregion Plane printing

        #region Cylinder printing

        // On affiche le cylindre
        if (cylinderObject != null)
        {
            cylinderObject.transform.position = (cylinder.P2 + cylinder.P1) / 2.0f;
            cylinderObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, (cylinder.P2 - cylinder.P1).normalized);
            cylinderObject.transform.localScale = new Vector3(cylinder.R * 2, Vector3.Distance(cylinder.P1, cylinder.P2) / 2, cylinder.R * 2);
        }

        #endregion Cylinder printing
    }

    public void OnDrawGizmos()
    {
        #region Point printing

        // On affiche le point
        Gizmos.DrawSphere(point, 0.3f); 


        // On affiche la distance point / droite
        Handles.Label(point + new Vector3(0, 3, 0), string.Format("Distance point / droite = {0:0.00}", line.GetDistance(point)), gUIStyle);
        Handles.Label(point + new Vector3(0, 2, 0), string.Format("Distance point / plan = {0:0.00}", plane.GetDistance(point)), gUIStyle);

        #endregion Segment printing

        #region Line printing

        // On affiche la droite
        Gizmos.DrawLine(line.P1, line.P2);

        #endregion Line printing

        #region Point printing

        // On affiche le segment
        Gizmos.DrawLine(segment.P1, segment.P2);

        #endregion Segment printing

        #region Plane printing

        // Intersections plan, segment
        if (planeObject != null)
        {
            Vector3 planeInterP, planeVectN;
            if (Intersection.InterSegPlane(segment, plane, out planeInterP, out planeVectN))
            {
                planeObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.red;
                DrawIntersectionInfos(planeInterP, planeVectN);
            }
            else
            {
                planeObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.blue;
            }
        }

        #endregion Plane color printing

        #region Sphere printing

        // Intersections sphere, segment
        Vector3 sphereInterP, sphereVectN;
        if (Intersection.InterSegSphere(segment, sphere, out sphereInterP, out sphereVectN))
        {
            DrawIntersectionInfos(sphereInterP, sphereVectN);
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        // On affiche la sphère
        Gizmos.DrawSphere(sphere.P, sphere.R);

        #endregion Sphere printing

        #region Cylinder printing

        // Intersections cylindre, segment
        if (cylinderObject != null)
        {
            Vector3 cylinderInterP, cylinderVectN;
            if (Intersection.InterSegCylinder(segment, cylinder, out cylinderInterP, out cylinderVectN))
            {
                cylinderObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.red;
                DrawIntersectionInfos(cylinderInterP, cylinderVectN);
            } else
            {
                cylinderObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.blue;
            }
        }

        #endregion Cylinder printing
    }


    ///<summary> Affiche le point d'intersection et le vecteur normal d'intersection de la forme par rapport au segment </summary>
    ///<param name="interP">Point d'intersection </param>
    ///<param name="vectN">Vecteur normal d'intersection</param>
    private void DrawIntersectionInfos(Vector3 interP, Vector3 vectN)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(interP, 0.5f);
        Gizmos.DrawLine(interP, interP + vectN);
    }

    #endregion Printing
}