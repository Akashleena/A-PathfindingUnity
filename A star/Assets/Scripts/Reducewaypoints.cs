//using System.Numerics;
// using System.Numerics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// Uses the Douglas Peucker algorithm to reduce the number of points.
/// </summary>
/// <param name="Points">The points.</param>
/// <param name="Tolerance">The tolerance.</param>
/// <returns></returns>

public class Reducewaypoints : MonoBehaviour
{
    Grid g;
    public float nodeRadius;
    public LayerMask unwalkableMask;

    public void Grid()
    {
        nodeRadius = g.nodeRadius;
        unwalkableMask = g.unwalkableMask;
    }




    public List<Vector3> DouglasPeuckerReduction
        (List<Vector3> Points, double Tolerance)
    {
        if (Points == null || Points.Count < 3)
            return Points;

        int firstPoint = 0;
        int lastPoint = Points.Count - 1;
        List<int> pointIndexsToKeep = new List<int>();

        //Add the first and last index to the keepers
        pointIndexsToKeep.Add(firstPoint);
        pointIndexsToKeep.Add(lastPoint);

        //The first and the last point cannot be the same
        while (Points[firstPoint].Equals(Points[lastPoint]))
        {
            lastPoint--;
        }

        DouglasPeuckerReduction(Points, firstPoint, lastPoint,
        Tolerance, ref pointIndexsToKeep);

        List<Vector3> returnPoints = new List<Vector3>();
        pointIndexsToKeep.Sort();

        foreach (int index in pointIndexsToKeep)
        {
            returnPoints.Add(Points[index]);
        }

        return returnPoints;
    }
    private bool CheckLinewalkable(Vector3 firstpoint, Vector3 secondpoint)
    {
        // nodeRadius = g.nodeRadius;
        // unwalkableMask = g.unwalkableMask;
        float slope = (Math.Abs(firstpoint.z - secondpoint.z)) / (Math.Abs(firstpoint.x - secondpoint.x));
        float dist = 0.1f;
        float xend = (float)(secondpoint.x + dist * (Math.Sqrt(1 / (1 + (slope * slope)))));
        float zend = (float)(secondpoint.y + slope * dist * (Math.Sqrt(1 / (1 + (slope * slope)))));
        float xnew = (float)(firstpoint.x + dist * (Math.Sqrt(1 / (1 + (slope * slope)))));
        float znew = (float)(firstpoint.y + slope * dist * (Math.Sqrt(1 / (1 + (slope * slope)))));
        float ynew = 0.0f;
        Vector3 newpoint = new Vector3(xnew, ynew, znew);
        bool notcollision = !(Physics.CheckSphere(newpoint, nodeRadius, unwalkableMask));
        if (notcollision == false)
        {
            return false;

        }
        else
        {
            if (!(xnew >= xend) && (znew) >= (zend))
                CheckLinewalkable(newpoint, secondpoint);
            return true;
        }
    }

    /// <summary>
    /// Douglases the peucker reduction.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="firstPoint">The first point.</param>
    /// <param name="lastPoint">The last point.</param>
    /// <param name="tolerance">The tolerance.</param>
    /// /// <param name="pointIndexsToKeep">The point index to keep.</param>
    private void DouglasPeuckerReduction(List<Vector3>
        points, int firstPoint, int lastPoint, double tolerance,
        ref List<int> pointIndexsToKeep)
    {
        double maxDistance = 0;
        int indexFarthest = 0;
        int prevIndex = 0;
        int nextIndex = 0;

        for (int index = firstPoint; index < lastPoint; index++)
        {
            double distance = PerpendicularDistance
                (points[firstPoint], points[lastPoint], points[index]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexFarthest = index;
                if (index - 1 != firstPoint)
                    prevIndex = index - 1;
                if (index + 1 != lastPoint)
                    nextIndex = index + 1;

            }
        }

        if (maxDistance > tolerance && indexFarthest != 0)
        {
            //Add the largest point that exceeds the tolerance

            /*get the previous point and next point
            //check if the line joining previous point and next point is collision free 
            //if collision free then call the recursion*/


            pointIndexsToKeep.Add(indexFarthest);
            if (CheckLinewalkable(points[firstPoint], points[indexFarthest]))
            {
                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
            }
            if (CheckLinewalkable(points[indexFarthest], points[lastPoint]))
            {
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }
    }

    /// <summary>
    /// The distance of a point from a line made from point1 and point2.
    /// </summary>
    /// <param name="pt1">The PT1.</param>
    /// <param name="pt2">The PT2.</param>
    /// <param name="p">The p.</param>
    /// <returns></returns>
    public static double PerpendicularDistance
        (Vector3 Point1, Vector3 Point2, Vector3 Point)
    {
        //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
        //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
        //Area = .5*Base*H                                          *Solve for height
        //Height = Area/.5/Base

        double area = Math.Abs(.5 * (Point1.x * Point2.z + Point2.x *
        Point.z + Point.x * Point1.z - Point2.x * Point1.z - Point.x *
        Point2.z - Point1.x * Point.z));
        double bottom = Math.Sqrt(Math.Pow(Point1.x - Point2.x, 2) +
        Math.Pow(Point1.z - Point2.z, 2));
        double height = area / bottom * 2;

        return height;

        //Another option
        //double A = Point.X - Point1.X;
        //double B = Point.Y - Point1.Y;
        //double C = Point2.X - Point1.X;
        //double D = Point2.Y - Point1.Y;

        //double dot = A * C + B * D;
        //double len_sq = C * C + D * D;
        //double param = dot / len_sq;

        //double xx, yy;

        //if (param < 0)
        //{
        //    xx = Point1.X;
        //    yy = Point1.Y;
        //}
        //else if (param > 1)
        //{
        //    xx = Point2.X;
        //    yy = Point2.Y;
        //}
        //else
        //{
        //    xx = Point1.X + param * C;
        //    yy = Point1.Y + param * D;
        //}

        //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
    }
}