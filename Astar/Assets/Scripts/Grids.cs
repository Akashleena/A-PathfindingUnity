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

    public Vector3 bottomLeftPoint, leftNode, bottomNode, bottomLeftNode;
    public float nodeRadius = 50;
    Node[,] grid;
    Node node;
    public Vector3 worldBottomLeft;
    public LineRenderer boundingBox;
    public LineRenderer obstacleRenderer = new LineRenderer();
    public Transform seeker;
    public Transform testPrefab;

    [Space]

    [Header("Node Parameters")]
    public float nodeDiameter;

    int obstacleGridX, obstacleGridY;
    public float[] perpDist;
    public int gridSizeX, gridSizeY;
    public Vector3[] polygon1;
    public Vector3[] boundingRectangle;
    public int n;
    static float INF = 10000.0f;
    public LineRenderer rightvectorarrow;
    public LineRenderer leftvectorarrow;

    public CheckifinsideObstacle co;
    void Awake()
    {
        // rightvectorarrow = gameObject.AddComponent<LineRenderer>();
        // leftvectorarrow = gameObject.AddComponent<LineRenderer>();
        node = new Node(true, Vector3.zero, 0, 0, false);


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

        for (int i = 0; i < gridSizeX; i++)
            for (int j = 0; j < gridSizeY; j++)
                grid[i, j] = new Node();

        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        bool walkable;
        Vector3 extremeright = new Vector3(INF, 0, 0);
        Vector3 extremeleft = new Vector3(-INF, 0, 0);
        rightvectorarrow.positionCount = 2;
        leftvectorarrow.positionCount = 2;

        // create obstacles and bounding box
        // polygon1 = new Vector3[6];
        // polygon1[0] = new Vector3(20f, 0f, -350f);
        // polygon1[1] = new Vector3(160f, 0f, -350f);
        // polygon1[2] = new Vector3(200f, 0f, 200f);
        // polygon1[3] = new Vector3(400f, 0, -100f);
        // polygon1[4] = new Vector3(300f, 0, -600f);
        // polygon1[5] = new Vector3(20f, 0, -350f);

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
        boundingRectangle[0] = new Vector3(minX - (2 * nodeDiameter), 0f, minZ - (2 * nodeDiameter));
        boundingRectangle[1] = new Vector3(maxX + (2 * nodeDiameter), 0f, minZ - (2 * nodeDiameter));
        boundingRectangle[2] = new Vector3(maxX + (2 * nodeDiameter), 0f, maxZ + (2 * nodeDiameter));
        boundingRectangle[3] = new Vector3(minX - (2 * nodeDiameter), 0f, maxZ + (2 * nodeDiameter));
        boundingBox.positionCount = 5;

        boundingBox.SetPosition(0, boundingRectangle[0]);
        boundingBox.SetPosition(1, boundingRectangle[1]);
        boundingBox.SetPosition(2, boundingRectangle[2]);
        boundingBox.SetPosition(3, boundingRectangle[3]);
        boundingBox.SetPosition(4, boundingRectangle[0]);
        // // boundingBox.SetPosition(5, boundingRectangle[1]);

        obstacleRenderer.positionCount = polygon1.Length;
        for (int i = 0; i < polygon1.Length; i++)
        {
            obstacleRenderer.SetPosition(i, polygon1[i]);
        }

        Vector3 obstaclegridmidpoint;
        //disable the grid corresponding to the vertex points of the polygon
        for (int i = 0; i < polygon1.Length; i++)
        {
            obstacleGridX = (int)((polygon1[i].x + 950) / nodeDiameter);
            obstacleGridY = (int)((polygon1[i].y + 950) / nodeDiameter);
            obstaclegridmidpoint.x = (Mathf.Floor(polygon1[i].x / nodeDiameter) * 100) + nodeRadius;
            obstaclegridmidpoint.z = (Mathf.Floor(polygon1[i].z / nodeDiameter) * 100) + nodeRadius;
            obstaclegridmidpoint.y = 0;
            // Debug.Log("obstaclemidpont" + obstaclegridmidpoint.x + " " + obstaclegridmidpoint.z);

            grid[obstacleGridX, obstacleGridY] = new Node(false, obstaclegridmidpoint, obstacleGridX, obstacleGridY, true);


            Vector3 objectPOS0 = obstaclegridmidpoint;
            Instantiate(testPrefab, objectPOS0, Quaternion.identity);

        }


        // Debug.Log("obstaclegridx" + obstacleGridX);
        // Debug.Log("obstaclegridy" + obstacleGridY);
        n = polygon1.Length;


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //make only inside the bounding box as unwalkable
                if (worldPoint.x >= (minX - (2.0 * nodeDiameter)) && worldPoint.x <= (maxX + (2.0f * nodeDiameter)) && worldPoint.z >= (minZ - (2 * nodeDiameter)) && worldPoint.z <= (maxZ + (2 * nodeDiameter)))
                {

                    bottomLeftPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeRadius);//botoom right
                    bottomNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter); //subtract node diameter from worldpoint.y
                    bottomLeftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter);
                    leftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                    rightvectorarrow.SetPosition(0, Vector3.zero);
                    rightvectorarrow.SetPosition(1, extremeright);
                    leftvectorarrow.SetPosition(0, Vector3.zero);
                    leftvectorarrow.SetPosition(1, extremeleft);

                    if (co.isInside(polygon1, n, bottomLeftPoint, extremeright))

                    {

                        walkable = false;
                        grid[x, y] = new Node(walkable, worldPoint, x, y, true);

                        grid[x, y - 1] = new Node(walkable, bottomNode, x, y - 1, true);

                        Debug.Log("worldpoint" + worldPoint + "walkable" + walkable);

                        Debug.Log("bottomnode" + bottomNode + "walkable" + walkable);

                        Vector3 objectPOS1 = worldPoint;
                        Instantiate(testPrefab, objectPOS1, Quaternion.identity);

                        Vector3 objectPOS2 = bottomNode;
                        Instantiate(testPrefab, objectPOS2, Quaternion.identity);


                    }

                    if (co.isInside(polygon1, n, bottomLeftPoint, extremeleft))
                    {


                        walkable = false;

                        grid[x - 1, y] = new Node(walkable, leftNode, x - 1, y, true);

                        grid[x - 1, y - 1] = new Node(walkable, bottomLeftNode, x - 1, y - 1, true);


                        Debug.Log("leftNode" + leftNode + "walkable" + walkable);

                        Debug.Log("bottomLeftNode" + bottomLeftNode + "walkable" + walkable);

                        Vector3 objectPOS3 = leftNode;
                        Instantiate(testPrefab, objectPOS3, Quaternion.identity);


                        Vector3 objectPOS4 = bottomLeftNode;
                        Instantiate(testPrefab, objectPOS4, Quaternion.identity);

                    }
                    else // lies inside bounding box and it is walkable 
                    {

                        // check if it was set non-walkable by some previous condition, if yes, let it remain non-walkable
                        node = grid[x, y];// worldpoint
                        //Debug.Log("worldpoint" + node.gridX + node.gridY);
                        if (node.walkable == false) //previously unwalkable
                            walkable = false;
                        else
                            walkable = true;



                        if (walkable == false)
                        {
                            Vector3 objectPOS5 = worldPoint;
                            Instantiate(testPrefab, objectPOS5, Quaternion.identity);
                            //  Debug.Log("else  world point" + worldPoint + "walkable" + walkable);
                        }

                        grid[x, y] = new Node(walkable, worldPoint, x, y, true);

                        //Debug.Log("else  world point" + worldPoint + "walkable" + walkable);
                        // check if it was set non-walkable by some previous condition, if yes, let it remain non-walkable
                        node = grid[x, y - 1]; //bottomnode
                        // Debug.Log("worldpoint" + node.gridX + node.gridY);

                        if (node.walkable == false)
                            walkable = false;
                        else
                            walkable = true;


                        if (walkable == false) //for visualization
                        {
                            Vector3 objectPOS6 = bottomNode;
                            Instantiate(testPrefab, objectPOS6, Quaternion.identity);
                            //  Debug.Log("else  bottom node" + bottomNode + "walkable" + walkable);

                        }


                        grid[x, y - 1] = new Node(walkable, bottomNode, x, y - 1, true);


                        node = grid[x - 1, y]; //leftNode
                        // Debug.Log("worldpoint" + node.gridX + node.gridY);

                        if (node.walkable == false)//previously not walkable
                            walkable = false;
                        else
                            walkable = true;


                        if (walkable == false)
                        {
                            Vector3 objectPOS7 = leftNode;
                            Instantiate(testPrefab, objectPOS7, Quaternion.identity);
                            // Debug.Log("else   left node" + leftNode + "walkable" + walkable);

                        }
                        grid[x - 1, y] = new Node(walkable, leftNode, x - 1, y, true);


                        node = grid[x - 1, y - 1]; //bottomLeftNode
                        // Debug.Log("worldpoint" + node.gridX + node.gridY);

                        if (node.walkable == false)//previously  walkable
                            walkable = false;
                        else
                            walkable = true;

                        if (walkable == false)
                        {
                            Vector3 objectPOS8 = bottomLeftNode;
                            Instantiate(testPrefab, objectPOS8, Quaternion.identity);
                            // Debug.Log("else  left node" + bottomLeftNode + "walkable" + walkable);

                        }

                        grid[x - 1, y - 1] = new Node(walkable, bottomLeftNode, x - 1, y - 1, true);


                    }
                }
                else //outside bounding box
                {
                    walkable = true;


                    //Debug.Log("worldpoint" + worldPoint + "is walkable" + walkable);


                    grid[x, y] = new Node(walkable, worldPoint, x, y, true);

                }


            }
        }
    }


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