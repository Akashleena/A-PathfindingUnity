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
    static float INF = 10000.0f;
    Node node;
    public bool walkable;
    private Transform testPrefab;
    Vector3 extremeright = new Vector3(INF, 0, 0);
    Vector3 extremeleft = new Vector3(-INF, 0, 0);
    void Start()
    {
       
      
    }

    private void CheckWalkableNodesinsideBoundingBox(Node[,] grid, int x, int y, Vector3 worldposition, List<Node> finalunwalkableNodes, String bbnode)
    {
        node = grid[x, y];// worldpoint

        if (node.walkable == false) //previously unwalkable
        {
            walkable = false;
            finalunwalkableNodes.Add(node);
        }
        else
            walkable = true;

        if (walkable == false)
        {
            Vector3 objectPOS6 = worldposition;
            var repeatNodes = Instantiate(testPrefab, objectPOS6, Quaternion.identity);
            repeatNodes.GetComponent<Renderer>().material.color = Color.red;

            //  Debug.Log("else  world point" + worldPoint + "walkable" + walkable);
        }

        grid[x, y] = new Node(walkable, worldposition, x, y, true);
    }


    private void Calculatefouradjnodes(int x, int y, Vector3 worldBottomLeft, float nodeDiameter, float nodeRadius)
    {
        bottomLeftPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeRadius);//bottom right
        bottomNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter); //subtract node diameter from worldpoint.y
        bottomLeftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter);
        leftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius);
    }

    public void IterateGrid(int gridSizeX, int gridSizeY, Vector3 worldBottomLeft, List<Grids.SerializableClass> obstacleList, List<Node> finalunwalkableNodes, Node[,]grid, float nodeDiameter, float nodeRadius)
    {
        co = gameObject.AddComponent<CheckifinsideObstacle>();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //Debug.Log("world point" + worldPoint);
                foreach (Grids.SerializableClass sc in obstacleList)
                {
                    if (worldPoint.x >= (sc.minX - (2.0 * nodeDiameter)) && worldPoint.x <= (sc.maxX + (2.0f * nodeDiameter)) && worldPoint.z >= (sc.minZ - (2 * nodeDiameter)) && worldPoint.z <= (sc.maxZ + (2 * nodeDiameter)))
                    {
                        Calculatefouradjnodes(x, y, worldBottomLeft, nodeDiameter, nodeRadius);
                    }
                    if (co.isInside(sc.polygon1, sc.polygon1.Count, bottomLeftPoint, extremeright))
                    {
                        walkable = false;

                        finalunwalkableNodes.Add(grid[x, y]);
                        finalunwalkableNodes.Add(grid[x, y - 1]);
                        grid[x, y] = new Node(walkable, worldPoint, x, y, true);
                        grid[x, y - 1] = new Node(walkable, bottomNode, x, y - 1, true);
                        #region Debug statements
                        //Debug.Log("worldpoint unwalkable right" + worldPoint);
                        //Debug.Log("bottomnode unwalkable right" + bottomNode);
                        #endregion

                        Vector3 objectPOS1 = worldPoint;
                        var insideObstacleworldPoint = Instantiate(testPrefab, objectPOS1, Quaternion.identity);
                        insideObstacleworldPoint.GetComponent<Renderer>().material.color = Color.red;

                        Vector3 objectPOS2 = bottomNode;
                        var insideObstaclebottomNode = Instantiate(testPrefab, objectPOS2, Quaternion.identity);
                        insideObstaclebottomNode.GetComponent<Renderer>().material.color = Color.red;


                    }
                    if (co.isInside(sc.polygon1, sc.polygon1.Count, bottomLeftPoint, extremeleft))
                    {
                        walkable = false;
                        grid[x - 1, y] = new Node(walkable, leftNode, x - 1, y, true);
                        grid[x - 1, y - 1] = new Node(walkable, bottomLeftNode, x - 1, y - 1, true);
                        finalunwalkableNodes.Add(grid[x - 1, y]);
                        finalunwalkableNodes.Add(grid[x - 1, y - 1]);
                        #region Debug statements
                        Debug.Log("leftNode unwalkable left vector" + leftNode);
                        Debug.Log("bottomLeftNode unwalkable left vector" + bottomLeftNode);
                        #endregion
                        Vector3 objectPOS3 = leftNode;
                        var insideObstacleleftnode =
                            Instantiate(testPrefab, objectPOS3, Quaternion.identity);
                        insideObstacleleftnode.GetComponent<Renderer>().material.color = Color.red;


                        Vector3 objectPOS4 = bottomLeftNode;
                        var insideObstaclebottomleftnode =
                            Instantiate(testPrefab, objectPOS4, Quaternion.identity);
                        insideObstaclebottomleftnode.GetComponent<Renderer>().material.color = Color.red;

                    }

                
                    else // lies inside bounding box and it is walkable 
                    {

                        CheckWalkableNodesinsideBoundingBox(grid,  x, y, worldPoint,finalunwalkableNodes, "worldpoint");
                        CheckWalkableNodesinsideBoundingBox(grid, x, (y - 1), bottomNode, finalunwalkableNodes, "bottomnode");
                        CheckWalkableNodesinsideBoundingBox(grid, (x - 1), y, leftNode, finalunwalkableNodes, "leftnode");
                        CheckWalkableNodesinsideBoundingBox(grid, (x - 1), (y - 1), bottomLeftNode, finalunwalkableNodes, "bottomleftnode");
                    }
                }
            }
        }
    }
}
    