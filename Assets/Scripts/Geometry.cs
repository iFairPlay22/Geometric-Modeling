using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
    [SerializeField]
    private GameObject planeObject;

    private Plane plane = new Plane(new Vector3(1, 0, 0), 1);

    private Segment segment = new Segment(new Vector3(1, 1, 1), new Vector3(11, 10, 11));
    private Sphere sphere = new Sphere(new Vector3(12, 10, 11), 2);

    private void Update()
    {
        // Update la position du plane
        planeObject.transform.LookAt(plane.N);
        planeObject.transform.position = plane.N * plane.D;
    }

    public void OnDrawGizmos()
    {
        if (planeObject == null) return;
        Gizmos.DrawLine(segment.P1, segment.P2);

        // Intersections plan, segment
        Vector3 planeInterP, planeVectN;
        planeObject.GetComponentInChildren<Renderer>().sharedMaterial.color = Intersection.InterSegPlane(segment, plane, out planeInterP, out planeVectN) ? Color.red : Color.blue;

        // Intersections sphere, segment
        Vector3 sphereInterP, sphereVectN;
        Gizmos.color = Intersection.InterSegSphere(segment, sphere, out sphereInterP, out sphereVectN) ? Color.red : Color.blue;
        Gizmos.DrawSphere(sphere.P, sphere.R);
    }
}