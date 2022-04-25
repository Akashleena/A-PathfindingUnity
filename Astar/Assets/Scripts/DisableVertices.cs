using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>
/// disable the grid corresponding to the vertex points of the polygon
/// <para>
/// <summary>

public class DisableVertices : MonoBehaviour
{
    public List<Vector3> DisablePolygonVertex(List<Vector3>polygon1, List<Vector3> unwalkableNodes)
    {

        for (int i = 0; i < polygon1.Count; i++)
        {
            
            //obstacleGridX = (int)((polygon1[i].x - worldBottomLeft.x) / nodeDiameter); //shift origin to (-950,-950)
            //obstacleGridY = (int)((polygon1[i].y) / nodeDiameter); //z axis origin need not be shifted
            //obstaclegridmidpoint.x = (Mathf.Floor(polygon1[i].x / nodeDiameter) * 100) + nodeRadius;
            //obstaclegridmidpoint.z = (Mathf.Floor(polygon1[i].z / nodeDiameter) * 100) + nodeRadius;
            //obstaclegridmidpoint.y = 0;

            unwalkableNodes.Add(polygon1[i]);
            // grid[obstacleGridX, obstacleGridY] = new Node(false, obstaclegridmidpoint, obstacleGridX, obstacleGridY, true);
            //dismantleobstaclenodes.Add(grid[obstacleGridX, obstacleGridY]);
            #region comments
            //Vector3 objectPOS0 = obstaclegridmidpoint;
            //var obstaclevertices = Instantiate(testPrefab, objectPOS0, Quaternion.identity);
            //obstaclevertices.GetComponent<Renderer>().material.color = Color.black; //remove the line renderers and disable only the vertex coordinates
            #endregion
           
            // display positions of obstacles vertex and not position of corresponding grid midpoints
            //Vector3 objectPOS0 = new Vector3(polygon1[i].x, polygon1[i].y, polygon1[i].z);
            //var obstaclevertices = Instantiate(testPrefab, objectPOS0, Quaternion.identity);
            //obstaclevertices.GetComponent<Renderer>().material.color = Color.black; //remove the line renderers and disable only the vertex coordinates

        }
        return unwalkableNodes;
    }

   
}