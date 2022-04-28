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
    List<float> bounds;
    public float nodeRadius = 50;
    public float nodeDiameter;
    public Node[,] grid;
    public Node node;  
   // public int noofobstacles = 1;
    public List<Vector3> unwalkableNodes = new List<Vector3>();
 
    [Header("DismantleObstacles")]
    public Vector3 worldBottomLeft;
    public LineRenderer obstacleRenderer = new LineRenderer(); 
    public Vector3 obstaclegridmidpoint;
    [Space]

    BoundingRectangle br;

    [Header("Node Parameters")]
      public int n;
    #endregion

    void Awake()
    {
       
        //BoundingRectangle br = GetComponent<BoundingRectangle>();
       
        node = new Node();
        
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        

    }
     void Start()
    {

    }
      public List<Vector3> CreateGrid(List<Vector3> polygon1, int obstacleid)
    {
        Debug.Log("Inside Create grid");
        grid = new Node[gridSizeX, gridSizeY];

        for (int i = 0; i < gridSizeX; i++)
            for (int j = 0; j < gridSizeY; j++)
                grid[i, j] = new Node();
        
            worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            BoundingRectangle br = gameObject.GetComponent<BoundingRectangle>();  
            List<float> bounds = br.CreateBoundingRectangle(polygon1, obstacleRenderer, obstacleid);
            var dv = gameObject.GetComponent<DisableVertices>();
            unwalkableNodes = dv.DisablePolygonVertex(polygon1, unwalkableNodes);
      
            var itg = gameObject.GetComponent<IterateThroughGrid>();
            unwalkableNodes = itg.IterateGrid(gridSizeX, gridSizeY, worldBottomLeft, polygon1, unwalkableNodes, grid, nodeDiameter, nodeRadius, bounds);

        return unwalkableNodes;

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