using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
    #region Attributes

    #region Plane attributes

    [SerializeField]
    private GameObject planeObject;

    private Plane plane = new Plane(new Vector3(10, 5, 10), 5);

    #endregion Plane attributes

    #region Segment attributes

    private Segment segment = new Segment(new Vector3(0, 0, 0), new Vector3(20, 0, 20));

    #endregion Segment attributes

    #region Sphere attributes

    private Sphere sphere = new Sphere(new Vector3(12, 0, 10), 2.5f);

    #endregion Sphere attributes

    #region Cylinder attributes

    [SerializeField]
    private GameObject cylinderObject;

    private Cylinder cylinder = new Cylinder(new Vector3(15, 0, 15), new Vector3(15, 5, 15), 2);

    #endregion Cylinder attributes

    #endregion Attributes

    #region Printing

    private void Update()
    {
        #region Plane printing

        // Update la position du plane
        if (planeObject != null)
        {
            // Rotate pla,e
            plane.N = (Quaternion.Euler(0, 15 * Time.deltaTime, 0) * plane.N).normalized;
            
            // Ddisplay plane
            planeObject.transform.LookAt(plane.N);
            planeObject.transform.position = plane.N * plane.D;
        }

        #endregion Plane printing

        #region Sphere printing

        // Update la position de la sphère
        sphere.P = (Quaternion.Euler(50 * Time.deltaTime, 0, 0) * sphere.P);

        #endregion Sphere printing

        #region Cylinder printing

        // Update la position du cylindre
        if (cylinderObject != null)
        {
            // Update la position de la sphère
            cylinder.P1 = Quaternion.Euler(20 * Time.deltaTime, 0, 0) * cylinder.P1;

            // Affichage de la sphère
            cylinderObject.transform.position = (cylinder.P2 + cylinder.P1) / 2.0f;
            cylinderObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, (cylinder.P2 - cylinder.P1).normalized);
            cylinderObject.transform.localScale = new Vector3(cylinder.R, Vector3.Distance(cylinder.P1, cylinder.P2) / 2, cylinder.R);
        }

        #endregion Cylinder printing
    }

    public void OnDrawGizmos()
    {
        #region Segment printing

        // On affiche le segment
        Gizmos.DrawLine(segment.P1, segment.P2);

        #endregion Segment printing

        #region Plane color printing

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
        Gizmos.DrawSphere(sphere.P, sphere.R);

        #endregion Sphere printing

        #region Cylinder color printing

        // Intersections cylindre, segment
        if (cylinderObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cylinder.P1, 1f);
            Gizmos.DrawSphere(cylinder.P2, 1f);

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

        #endregion Cylinder color printing
    }

    private void DrawIntersectionInfos(Vector3 cylinderInterP, Vector3 cylinderVectN)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(cylinderInterP, 0.2f);
        Gizmos.DrawLine(cylinderInterP, cylinderInterP + cylinderVectN);
    }

    #endregion Printing
}