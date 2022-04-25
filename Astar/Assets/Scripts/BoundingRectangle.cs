﻿using UnityEngine;
using System.Collections.Generic;
public  class BoundingRectangle : MonoBehaviour
{


    /// <summary>
    /// <para>
    /// create a bounding rectangle around the polygon obstacle with sides ranging from minX-2, maxX+2, minZ-2, maxZ+2
    /// </para>
    /// </summary>
    float minX = 10000, minZ = 10000;
    float maxX = 0, maxZ = 0;
    public List<float> bounds = new List<float>();
    public List<float> CreateBoundingRectangle(List<Vector3> polygon1, LineRenderer obstacleRenderer)
    {

        for (int i = 0; i < polygon1.Count; i++)
        {
           
            if (minX > polygon1[i].x)
                minX = polygon1[i].x;
            if (minZ > polygon1[i].z)
                minZ = polygon1[i].z;
            if (maxX < polygon1[i].x)
                maxX = polygon1[i].x;
            if (maxZ < polygon1[i].z)
                maxZ = polygon1[i].z;   
        }

        bounds.Add(minX);
        bounds.Add(maxX);
        bounds.Add(minZ);
        bounds.Add(maxZ);

        // obstacleRenderer.positionCount = polygon1.Count;
        //for (int i = 0; i < polygon1.Count; i++)
        //{
        //    obstacleRenderer.SetPosition(i, polygon1[i]);
        //}

        return bounds;

    }
}