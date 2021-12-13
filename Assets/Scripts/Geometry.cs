using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
    #region Attributes

    #region Segment attributes

    private Segment segment = new Segment(new Vector3(0, 0, 0), new Vector3(35, 0, 0));

    #endregion Segment attributes

    #region Plane attributes

    [SerializeField]
    private GameObject planeObject;

    private Plane plane = new Plane(new Vector3(10, 5, 10), 5);

    #endregion Plane attributes

    #region Sphere attributes

    private Sphere sphere = new Sphere(new Vector3(15, 0, 0), 1.5f);

    #endregion Sphere attributes

    #region Cylinder attributes

    [SerializeField]
    private GameObject cylinderObject;

    private Cylinder cylinder = new Cylinder(new Vector3(30, -2, 0), new Vector3(30, 5, 0), 1.5f);

    #endregion Cylinder attributes

    #endregion Attributes

    #region Printing

    private Vector3 kVect = new Vector3(1, 1, 1);
    private void Update()
    {
        #region Segment printing

        // On bouge le segment
        if (segment.P1.x < -10) kVect.x = 1;
        else if (10 < segment.P1.x) kVect.x = -1;

        if (segment.P1.y < -5) kVect.y = 1;
        else if (5 < segment.P1.y) kVect.y = -1;
        
        if (segment.P1.z < -2) kVect.z = 1;
        else if (2 < segment.P1.z) kVect.z = -1;

        Vector3 moveV = kVect * Time.deltaTime;
        segment.P1 += moveV;
        segment.P2 += moveV;

        #endregion

        #region Plane printing

        // Display plane
        if (planeObject != null)
        {
            planeObject.transform.LookAt(plane.N);
            planeObject.transform.position = plane.N * plane.D;
        }

        #endregion Plane printing

        #region Cylinder printing

        // Display cylinder
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
        #region Segment printing

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

        #endregion Cylinder color printing
    }

    private void DrawIntersectionInfos(Vector3 cylinderInterP, Vector3 cylinderVectN)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(cylinderInterP, 0.5f);
        Gizmos.DrawLine(cylinderInterP, cylinderInterP + cylinderVectN);
    }

    #endregion Printing
}