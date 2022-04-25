using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>
/// Iterate through grid nodes and sets walkability status of each node in the grid
/// <para>
/// <summary>

public class IterateThroughGrid : MonoBehaviour
{
    public Vector3 bottomLeftPoint, leftNode, bottomNode, bottomLeftNode;
    public CheckifinsideObstacle co;
    public BoundingRectangle bor;
    static float INF = 10000.0f;
    Node node;
    public bool walkable;
    private Transform testPrefab;
    Vector3 extremeright = new Vector3(INF, 0, 0);
    Vector3 extremeleft = new Vector3(-INF, 0, 0);
    void Start()
    {


    }
    //reduntant function
    private void CheckWalkableNodesinsideBoundingBox(Node[,] grid, int x, int y, Vector3 worldposition, List<Vector3> unwalkableNodes, String bbnode)
    {
        node = grid[x, y];// worldpoint

        if (node.walkable == false) //previously unwalkable
        {
            walkable = false;
            unwalkableNodes.Add(worldposition);
        }


    }



    private void Calculatefouradjnodes(int x, int y, Vector3 worldBottomLeft, float nodeDiameter, float nodeRadius)
    {
        bottomLeftPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeRadius);//bottom right
        bottomNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter); //subtract node diameter from worldpoint.y
        bottomLeftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter);
        leftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius);
    }

    public List<Vector3> IterateGrid(int gridSizeX, int gridSizeY, Vector3 worldBottomLeft, List<Vector3> polygon1, List<Vector3> unwalkableNodes, Node[,] grid, float nodeDiameter, float nodeRadius, List<float> bounds)
    {
        co = gameObject.AddComponent<CheckifinsideObstacle>();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //make only inside the bounding box as unwalkable everything outside bb will be walkable
                if (worldPoint.x >= (bounds[0] - (2.0 * nodeDiameter)) && worldPoint.x <= (bounds[1] + (2.0f * nodeDiameter)) && worldPoint.z >= (bounds[2] - (2 * nodeDiameter)) && worldPoint.z <= (bounds[3] + (2 * nodeDiameter)))
                {
                    Calculatefouradjnodes(x, y, worldBottomLeft, nodeDiameter, nodeRadius);

                    if (co.isInside(polygon1, polygon1.Count, bottomLeftPoint, extremeright))
                    {
                        walkable = false;
                        unwalkableNodes.Add(worldPoint);
                        unwalkableNodes.Add(bottomNode);

                      
            
                    }

                    if (co.isInside(polygon1, polygon1.Count, bottomLeftPoint, extremeleft))
                    {
                        walkable = false;
                        unwalkableNodes.Add(leftNode);
                        unwalkableNodes.Add(bottomLeftNode);

                    }

                    else // lies inside bounding box and it is walkable 
                    {

                        CheckWalkableNodesinsideBoundingBox(grid, x, y, worldPoint, unwalkableNodes, "worldpoint");
                        CheckWalkableNodesinsideBoundingBox(grid, x, (y - 1), bottomNode, unwalkableNodes, "bottomnode");
                        CheckWalkableNodesinsideBoundingBox(grid, (x - 1), y, leftNode, unwalkableNodes, "leftnode");
                        CheckWalkableNodesinsideBoundingBox(grid, (x - 1), (y - 1), bottomLeftNode, unwalkableNodes, "bottomleftnode");
                    }
                }
            }
        }
        return unwalkableNodes;
    }
}

          
            