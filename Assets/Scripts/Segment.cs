using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField]
    private Vector3 p1;

    public Vector3 P1
    {
        get { return p1; }
        private set { p1 = value; }
    }

    [SerializeField]
    private Vector3 p2;

    public Vector3 P2
    {
        get { return p2; }
        private set { p2 = value; }
    }

    public void OnDrawGizmos()
    {
        Vector3 p, n;

        if (Intersection.InterSegPlane(this, FindObjectOfType<Plane>(), out p, out n))
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;

        Gizmos.DrawLine(p1, p2);
    }
}