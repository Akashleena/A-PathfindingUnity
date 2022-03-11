using System.Xml;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;
public class Grids : MonoBehaviour
{
    [Header("Grid Parameters")]
    public LayerMask unwalkableMask;
    // public bool walkable;
    public Vector2 gridWorldSize;

    public Vector3 bottomRight, bottomLeft, topLeft, topRight;
    public float nodeRadius = 50;
    Node[,] grid;
    public Vector3 worldBottomLeft;
    public LineRenderer boundingBox;
    public LineRenderer obstacleRenderer = new LineRenderer();
    public Transform seeker;
    public Transform testPrefab;

    [Space]

    [Header("Node Parameters")]
    public float nodeDiameter;


    public float[] perpdist;
    public int gridSizeX, gridSizeY;
    public Vector3[] polygon1;
    public Vector3[] boundingRectangle;
    public int n;

    public CheckifinsideObstacle co;
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }
    void Start()
    {

    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        bool walkable;

        // create obstacles and bounding box
        polygon1 = new Vector3[6];
        polygon1[0] = new Vector3(20f, 0f, -350f);
        polygon1[1] = new Vector3(160f, 0f, -350f);
        polygon1[2] = new Vector3(200f, 0f, 200f);
        polygon1[3] = new Vector3(400f, 0, -100f);
        polygon1[4] = new Vector3(300f, 0, -600f);
        polygon1[5] = new Vector3(20f, 0, -350f);

        boundingRectangle = new Vector3[4];
        float maxZ = 0.0f, maxX = 0.0f;
        float minX = 10000f, minZ = 10000f;
        for (int i = 0; i < polygon1.Length; i++)
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
        boundingRectangle[0] = new Vector3(minX - nodeDiameter, 0f, minZ - nodeDiameter);
        boundingRectangle[1] = new Vector3(maxX + nodeDiameter, 0f, minZ - nodeDiameter);
        boundingRectangle[2] = new Vector3(maxX + nodeDiameter, 0f, maxZ + nodeDiameter);
        boundingRectangle[3] = new Vector3(minX - nodeDiameter, 0f, maxZ + nodeDiameter);
        boundingBox.positionCount = 5;

        boundingBox.SetPosition(0, boundingRectangle[0]);
        boundingBox.SetPosition(1, boundingRectangle[1]);
        boundingBox.SetPosition(2, boundingRectangle[2]);
        boundingBox.SetPosition(3, boundingRectangle[3]);
        boundingBox.SetPosition(4, boundingRectangle[0]);
        // boundingBox.SetPosition(5, boundingRectangle[1]);

        obstacleRenderer.positionCount = 6;
        obstacleRenderer.SetPosition(0, polygon1[0]);
        obstacleRenderer.SetPosition(1, polygon1[1]);
        obstacleRenderer.SetPosition(2, polygon1[2]);
        obstacleRenderer.SetPosition(3, polygon1[3]);
        obstacleRenderer.SetPosition(4, polygon1[4]);
        obstacleRenderer.SetPosition(5, polygon1[5]);

        n = polygon1.Length;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                if (worldPoint.x >= minX && worldPoint.x <= maxX && worldPoint.z >= minZ && worldPoint.z <= maxZ)
                {
                    bottomLeft = worldBottomLeft + Vector3.right * (x * nodeDiameter) + Vector3.forward * (y * nodeDiameter); //bottom left
                    bottomRight = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeDiameter) + Vector3.forward * (y * nodeDiameter);//botoom right
                    topLeft = worldBottomLeft + Vector3.right * (x * nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeDiameter);
                    topRight = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeDiameter);

                    Debug.Log("bottomLeft" + bottomLeft);

                    Debug.Log("bottomRight" + bottomRight);
                    Debug.Log("topRight" + topRight);
                    Debug.Log("topleft" + topLeft);

                    if (co.isInside(polygon1, n, topRight) ||
                      co.isInside(polygon1, n, bottomRight) ||
                    co.isInside(polygon1, n, topLeft) ||
                        co.isInside(polygon1, n, bottomLeft))
                    {

                        walkable = false;
                        Debug.Log("1");
                    }

                    else
                    {
                        walkable = true;
                    }
                }
                else
                    walkable = true;

                Debug.Log("worldpoint" + worldPoint + "is walkable" + walkable);


                grid[x, y] = new Node(walkable, worldPoint, x, y);
                if (walkable == false)
                {

                    Vector3 objectPOS = worldPoint;
                    Instantiate(testPrefab, objectPOS, Quaternion.identity);

                }
            }
        }
    }

    // public bool CheckInsidePolygon(Vector3 worldPoint)
    // {



    //     //Debug.Log("n" + n);
    //     if (co.isInside(polygon1, n, worldPoint))
    //     {
    //         // Debug.Log("Inside obstacle");
    //         return true;
    //     }
    //     else
    //         return false;
    // }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    public List<Node> path;

}