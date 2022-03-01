using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GrahamScan : MonoBehaviour
{
    const int TURN_LEFT = 1;
    const int TURN_RIGHT = -1;
    const int TURN_NONE = 0;
    public int turn(Vector3 p, Vector3 q, Vector3 r)
    {
        return ((q.x - p.x) * (r.z - p.z) - (r.x - p.x) * (q.z - p.z)).CompareTo(0);
    }

    public void keepLeft(List<Vector3> hull, Vector3 r)
    {
        while (hull.Count > 1 && turn(hull[hull.Count - 2], hull[hull.Count - 1], r) != TURN_LEFT)
        {
            Debug.Log("Removing Vector3 ({0}, {1}) because turning right " + hull[hull.Count - 1].x + hull[hull.Count - 1].z);
            hull.RemoveAt(hull.Count - 1);
        }
        if (hull.Count == 0 || hull[hull.Count - 1] != r)
        {
            Debug.Log("Adding Vector3 ({0}, {1})" + r.x + r.z);
            hull.Add(r);
        }
        Debug.Log("# Current Convex Hull #");
        foreach (Vector3 value in hull)
        {
            Debug.Log("(" + value.x + "," + value.z + ") ");
        }
        // Console.WriteLine();
        // Console.WriteLine();

    }

    public double getAngle(Vector3 p1, Vector3 p2)
    {
        float xDiff = p2.x - p1.x;
        float yDiff = p2.z - p1.z;
        return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
    }

    public List<Vector3> MergeSort(Vector3 p0, List<Vector3> arrPoint)
    {
        if (arrPoint.Count == 1)
        {
            return arrPoint;
        }
        List<Vector3> arrSortedInt = new List<Vector3>();
        int middle = (int)arrPoint.Count / 2;
        List<Vector3> leftArray = arrPoint.GetRange(0, middle);
        List<Vector3> rightArray = arrPoint.GetRange(middle, arrPoint.Count - middle);
        leftArray = MergeSort(p0, leftArray);
        rightArray = MergeSort(p0, rightArray);
        int leftptr = 0;
        int rightptr = 0;
        for (int i = 0; i < leftArray.Count + rightArray.Count; i++)
        {
            if (leftptr == leftArray.Count)
            {
                arrSortedInt.Add(rightArray[rightptr]);
                rightptr++;
            }
            else if (rightptr == rightArray.Count)
            {
                arrSortedInt.Add(leftArray[leftptr]);
                leftptr++;
            }
            else if (getAngle(p0, leftArray[leftptr]) < getAngle(p0, rightArray[rightptr]))
            {
                arrSortedInt.Add(leftArray[leftptr]);
                leftptr++;
            }
            else
            {
                arrSortedInt.Add(rightArray[rightptr]);
                rightptr++;
            }
        }
        return arrSortedInt;
    }

    public void convexHull(List<Vector3> points)
    {
        //Console.WriteLine("# List of Point #");
        // foreach (Vector3 value in points)
        // {
        //     Console.Write("(" + value.getX() + "," + value.getY() + ") ");
        // }
        // Console.WriteLine();
        // Console.WriteLine();
        // Vector3 p0 = null;
        Vector3 p0 = new Vector3();
        foreach (Vector3 value in points)
        {
            if (p0 == null)
                p0 = value;
            else
            {
                if (p0.z > value.z)
                    p0 = value;
            }
        }
        List<Vector3> order = new List<Vector3>();
        foreach (Vector3 value in points)
        {
            if (p0 != value)
                order.Add(value);
        }

        order = MergeSort(p0, order);
        // Console.WriteLine("# Sorted points based on angle with point p0 ({0},{1})#", p0.getX(), p0.getY());
        foreach (Vector3 value in order)
        {
            Debug.Log("(" + value.x + "," + value.z + ") : {0}" + getAngle(p0, value));
        }
        List<Vector3> result = new List<Vector3>();
        result.Add(p0);
        result.Add(order[0]);
        result.Add(order[1]);
        order.RemoveAt(0);
        order.RemoveAt(0);
        Debug.Log("# Current Convex Hull #");
        foreach (Vector3 value in result)
        {
            Console.Write("(" + value.x + "," + value.z + ") ");
        }
        // Console.WriteLine();
        // Console.WriteLine();
        foreach (Vector3 value in order)
        {
            keepLeft(result, value);
        }
        // Console.WriteLine();
        Console.WriteLine("# Convex Hull #");
        foreach (Vector3 value in result)
        {
            Debug.Log("(" + value.x + "," + value.z + ") ");
        }

    }

}

