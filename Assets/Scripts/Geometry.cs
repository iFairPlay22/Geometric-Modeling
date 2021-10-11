using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
    #region Attributes

    #region Plane attributes

    [SerializeField]
    private GameObject planeObject;

    private Plane plane = new Plane(new Vector3(1, 0, 0), 1);

    #endregion Plane attributes

    #region Segment attributes

    private Segment segment = new Segment(new Vector3(1, 1, 1), new Vector3(11, 10, 11));

    #endregion Segment attributes

    #region Sphere attributes

    private Sphere sphere = new Sphere(new Vector3(12, 10, 11), 2);

    #endregion Sphere attributes

    #region Cylinder attributes

    [SerializeField]
    private GameObject cylinderObject;

    private Cylinder cylinder = new Cylinder(new Vector3(1, 0, 0), new Vector3(10, 5, 6), 3);

    #endregion Cylinder attributes

    #endregion Attributes

    #region Printing

    private void Update()
    {
        #region Plane printing

        // Update la position du plane
        if (planeObject != null)
        {
            planeObject.transform.LookAt(plane.N);
            planeObject.transform.position = plane.N * plane.D;
        }

        #endregion Plane printing

        #region Cylinder printing

        // Update la position du cylindre
        if (cylinderObject != null)
        {
            cylinderObject.transform.LookAt(cylinder.P2);
            cylinderObject.transform.position = cylinder.P1;
            cylinderObject.transform.localScale = new Vector3(cylinder.R, cylinder.P2.y - cylinder.P1.y, cylinder.R);
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
            planeObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Intersection.InterSegPlane(segment, plane, out planeInterP, out planeVectN) ? Color.red : Color.blue;
        }

        #endregion Plane color printing

        #region Sphere printing

        // Intersections sphere, segment
        Vector3 sphereInterP, sphereVectN;
        Gizmos.color = Intersection.InterSegSphere(segment, sphere, out sphereInterP, out sphereVectN) ? Color.red : Color.blue;
        Gizmos.DrawSphere(sphere.P, sphere.R);

        #endregion Sphere printing

        #region Cylinder color printing

        // Intersections cylindre, segment
        if (cylinderObject != null)
        {
            Vector3 cylinderInterP, cylinderVectN;
            cylinderObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Intersection.InterSegCylinder(segment, cylinder, out cylinderInterP, out cylinderVectN) ? Color.red : Color.blue;
        }

        #endregion Cylinder color printing
    }

    #endregion Printing
}