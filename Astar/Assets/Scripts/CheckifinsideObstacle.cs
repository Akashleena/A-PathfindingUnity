// A C# program to check if a given point
// lies inside a given polygon
//https://www.geeksforgeeks.org/how-to-check-if-a-given-point-lies-inside-a-polygon/
// for explanation of functions onSegment(),
// orientation() and doIntersect()
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI; //for vectorStatus
using Debug = UnityEngine.Debug;

public class CheckifinsideObstacle : MonoBehaviour
{

    // Define Infinite (Using INT_MAX
    // caused overflow problems)
    // static float INF = 1000000.0f;
    public Text vectorStatus;
    Vector3 extreme;
    //public LineRenderer vectorarrow;

    // void Awake()
    // {
    //     vectorarrow = gameObject.AddComponent<LineRenderer>(); //to initialize LineRenderer in runtime
    // }




    // Given three collinear points p, q, r,
    // the function checks if Vector3 q lies
    // on line segment 'pr'
    static bool onSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Math.Max(p.x, r.x) &&
            q.x >= Math.Min(p.x, r.x) &&
            q.z <= Math.Max(p.z, r.z) &&
            q.z >= Math.Min(p.z, r.z))
        {
            return true;
        }
        return false;
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are collinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    private int orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        // https://www.geeksforgeeks.org/orientation-3-ordered-points/
        float val = (q.z - p.z) * (r.x - q.x) -
                (q.x - p.x) * (r.z - q.z);

        if (val == 0)
        {
            return 0; // collinear
        }
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    // The function that returns true if
    // line segment 'p1q1' and 'p2q2' intersect.
    private bool doIntersect(Vector3 p1, Vector3 q1,
                            Vector3 p2, Vector3 q2)
    {
        // https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        // Find the four orientations needed for
        // general and special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        // Special Cases
        // p1, q1 and p2 are collinear and
        // p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1))
        {
            return true;
        }

        // p1, q1 and p2 are collinear and
        // q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1))
        {
            return true;
        }

        // p2, q2 and p1 are collinear and
        // p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2))
        {
            return true;
        }

        // p2, q2 and q1 are collinear and
        // q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2))
        {
            return true;
        }

        // Doesn't fall in any of the above cases
        return false;
    }

    // Returns true if the Vector3 p lies
    // inside the polygon[] with n vertices
    public bool isInside(Vector3[] polygon, int n, Vector3 p, Vector3 extreme)
    {
        // There must be at least 3 vertices in polygon[]
        if (n < 3)
        {
            return false;
        }

        // Vector3 extreme = new Vector3(INF, 0, 0);
        // vectorStatus.text = "vector3 p.z is " + 0;

        // Vector3 extreme1 = new Vector3(-INF, 0, 0);


        // Debug.Log("e x t r e m e " + extreme);
        // vectorarrow.SetPosition(0, extreme);



        // with sides of polygon
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to
            // 'extreme' intersects with the line
            // segment from 'polygon[i]' to 'polygon[next]'
            if (doIntersect(polygon[i],
                            polygon[next], p, extreme))
            {
                // If the Vector3 'p' is collinear with line
                // segment 'i-next', then check if it lies
                // on segment. If it lies, return true, otherwise false
                if (orientation(polygon[i], p, polygon[next]) == 0)
                {
                    // Debug.Log("C . O . L .  L . I.  N.  E.  A . R ");
                    return onSegment(polygon[i], p,
                                    polygon[next]);
                }
                count++;
            }
            i = next;
        } while (i != 0);
        // Debug.Log("check count odd or even" + count);
        // Return true if count is odd, false otherwise
        return (count % 2 == 1); // Same as (count%2 == 1)
    }
}
