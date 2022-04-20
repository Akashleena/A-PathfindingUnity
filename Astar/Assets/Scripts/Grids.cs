using UnityEngine;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class Grids : MonoBehaviour
{
    #region declare variable
    [Header("Grid Parameters")]
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    int obstacleGridX, obstacleGridY;
    public float[] perpDist;
    public int gridSizeX, gridSizeY;

 
    public float nodeRadius = 50;
    public float nodeDiameter;
    public Node[,] grid;
    Node node;  
    public int noofobstacles = 1;

    [Header("DismantleObstacles")]
    public List<Node> finalunwalkableNodes;

    public Vector3 worldBottomLeft;
    public Transform testPrefab;
    public LineRenderer obstacleRenderer = new LineRenderer(); 
    public Transform seeker;
    public Vector3 obstaclegridmidpoint;
    [Space]

    [Header("Node Parameters")]
      public int n;
    #endregion

    [Serializable]
    public class SerializableClass
    {

        public List<Vector3> polygon1 = new List<Vector3>();
        public float maxZ = 0.0f, maxX = 0.0f;
        public float minX = 10000f, minZ = 10000f;
     
    }
    public List<SerializableClass> obstacleList = new List<SerializableClass>();

    void Awake()
    {
       
        
        node = new Node(true, Vector3.zero, 0, 0, false);
        finalunwalkableNodes = new List<Node>();
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
    

        int k = 0;
        foreach (SerializableClass sc in obstacleList)
        {
            // Debug.Log("value of sc" + sc);

            BoundingRectangle br = gameObject.AddComponent<BoundingRectangle>();
            br.CreateBoundingRectangle(sc, obstacleRenderer);

            var dv = gameObject.AddComponent<DisableVertices>();
            dv.DisablePolygonVertex(sc, grid, worldBottomLeft,nodeDiameter, nodeRadius,
                obstacleGridX, obstacleGridY, obstaclegridmidpoint, finalunwalkableNodes, testPrefab);
            k++;
           // Debug.Log("value of k" + k);
        }

        var itg = gameObject.AddComponent<IterateThroughGrid>();
        itg.IterateGrid(gridSizeX, gridSizeY, worldBottomLeft, obstacleList, finalunwalkableNodes, grid, nodeDiameter, nodeRadius);



        //for (int x=0; x< gridSizeX; x++)
        //{
        //    for (int y=0; y<gridSizeY; y++)
        //    {
        //        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
        //        for (int i=0; i<dismantleobstaclenodes.Count; i++)
        //        {
        //            if (worldPoint==dismantleobstaclenodes[i].worldPosition)
        //            {
        //                walkable = false;
        //                grid[x, y] = new Node(walkable, worldPoint, x, y, true);

        //                Vector3 objectPOS5 = worldPoint;
        //                    var obstacleprefab = Instantiate(testPrefab, objectPOS5, Quaternion.identity);
        //                    obstacleprefab.GetComponent<Renderer>().material.color = Color.red;

        //            }
        //            else
        //            {
        //                walkable = true;
        //                grid[x, y] = new Node(walkable, worldPoint, x, y, true);
        //            }

        //        }
        //    }
        //}


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