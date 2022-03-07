using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grids : MonoBehaviour
{
    [Header("Grid Parameters")]
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    public Vector3 worldBottomLeft;
    public LineRenderer obstacleRenderer = new LineRenderer();
    public Transform seeker;

    [Space]

    [Header("Node Parameters")]
    public float nodeDiameter;

    // public bool walkable;
    // public bool gridliesonpolygonside;

    public float[] perpdist;
    public int gridSizeX, gridSizeY;
    public Vector3[] polygon1;
    public int n;

    //LineRenderer pathLineRenderer;
    public CheckifinsideObstacle co;
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        // pathLineRenderer = new LineRenderer();
    }
    void Start()
    {
        // lpi = new Lineandpolygonintersection();
        //  lpi.checkLinePolyintersection();
    }



    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        polygon1 = new Vector3[6];
        polygon1[0] = new Vector3(20f, 0f, -350f);
        polygon1[1] = new Vector3(160f, 0f, -350f);
        polygon1[2] = new Vector3(200f, 0f, 200f);
        polygon1[3] = new Vector3(400f, 0, -100f);
        polygon1[4] = new Vector3(300f, 0, -600f);
        polygon1[5] = new Vector3(20f, 0, -350f);

        obstacleRenderer.positionCount = 6;
        obstacleRenderer.SetPosition(0, polygon1[0]);
        obstacleRenderer.SetPosition(1, polygon1[1]);
        obstacleRenderer.SetPosition(2, polygon1[2]);
        obstacleRenderer.SetPosition(3, polygon1[3]);
        obstacleRenderer.SetPosition(4, polygon1[4]);
        obstacleRenderer.SetPosition(5, polygon1[5]);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                // Debug.Log("worldPoint outside function" + worldPoint);

                // bool gridliesonpolygonside = PerpendicularDistance(worldPoint);
                // bool walkable = !((CheckInsidePolygon(worldPoint)) || (gridliesonpolygonside));
                bool walkable = !((CheckInsidePolygon(worldPoint)));
                Debug.Log("worldpoint" + worldPoint + "is walkable" + walkable);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // mesh.Clear();
    // mesh.vertices = new Vector3[] { new Vector3(12.5f, 0f, 60f), new Vector3(40.6f, 0f, 90f), new Vector3(250f, 0f, 300f), new Vector3(110f, 0, 200f), new Vector3(320f, 0, 1000f) };
    //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
    //mesh.triangles = new int[] { 0, 1, 2 };
    public bool PerpendicularDistance(Vector3 worldPoint)
    {
        perpdist = new float[polygon1.Length];
        // perpdist = new float[] { 10000, 10000, 10000, 10000, 10000, 10000 };
        int k = 0, T = 0, u = 0;
        for (int i = 0; i < polygon1.Length - 1; i++)
        {
            // Debug.Log("World point inside loop" + worldPoint);
            float slope = (polygon1[i + 1].z - polygon1[i].z) / (polygon1[i + 1].x - polygon1[i].x);
            //Debug.Log(polygon1[i + 1].z + " " + polygon1[i].z + " " + polygon1[i + 1].x + " " + polygon1[i].x);
            float num = Mathf.Abs(-(worldPoint.z - polygon1[i].z) + (slope * (worldPoint.x - polygon1[i].x)));
            // Debug.Log("num" + num);

            float den = Mathf.Sqrt(1 + Mathf.Pow(slope, 2));
            //Debug.Log("den" + den);
            perpdist[k] = num / den;
            // Debug.Log("Perpendicular distance" + perpdist[k]);
            k++;
            // Debug.Log("VALUE OF I" + i);
        }
        for (int i = 0; i < perpdist.Length; i++)
        {
            // Debug.Log("Perpendicular distance" + perpdist[i]);
            if (perpdist[i] < (nodeRadius * 1.414))
            {

                T = 1;
                u += 1;
            }

        }
        if (T == 1)
        {
            //Debug.Log("inside pd func" + u);
            return true;
        }
        else
            return false;
    }

    public bool CheckInsidePolygon(Vector3 worldPoint)
    {


        polygon1 = new Vector3[6];
        polygon1[0] = new Vector3(20f, 0f, -350f);
        polygon1[1] = new Vector3(160f, 0f, -350f);
        polygon1[2] = new Vector3(200f, 0f, 200f);
        polygon1[3] = new Vector3(400f, 0, -100f);
        polygon1[4] = new Vector3(300f, 0, -600f);
        polygon1[5] = new Vector3(20f, 0, -350f);

        obstacleRenderer.positionCount = 6;
        obstacleRenderer.SetPosition(0, polygon1[0]);
        obstacleRenderer.SetPosition(1, polygon1[1]);
        obstacleRenderer.SetPosition(2, polygon1[2]);
        obstacleRenderer.SetPosition(3, polygon1[3]);
        obstacleRenderer.SetPosition(4, polygon1[4]);
        obstacleRenderer.SetPosition(5, polygon1[5]);



        // Mesh mesh = GetComponent<MeshFilter>().mesh;
        // mesh.Clear();
        // mesh.vertices = new Vector3[]{new Vector3(100f, 0f, -350f),
        //                     new Vector3(120f, 0f, -350f),
        //                     new Vector3(50f, 0f, 200f),
        //                     new Vector3(110f, 0, 200f),
        //                     new Vector3(80f, 0, 320f)};
        n = polygon1.Length;
        //Debug.Log("n" + n);
        co = new CheckifinsideObstacle();
        if (co.isInside(polygon1, n, worldPoint))
        {
            // Debug.Log("Inside obstacle");
            return true;


        }
        else
            return false;

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

    // void RenderPath()
    // {
    //     Renderer rend;
    //     if (grid != null)
    //     {
    //         foreach (Node n in grid)
    //         {
    //             rend = seeker.GetComponent<Renderer>();
    //             rend.material.color = (n.walkable) ? Color.white : Color.black;
    //             if (path != null)
    //                 if (path.Contains(n))
    //                     rend.material.color = Color.magenta;
    //         }
    //     }

    // }
    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

    //     if (grid != null)
    //     {
    //         foreach (Node n in grid)
    //         {
    //             Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //             if (path != null)
    //                 if (path.Contains(n))
    //                 {
    //                     //UnityEngine.Debug.Log(n);
    //                     UnityEngine.Debug.Log(path);
    //                     Gizmos.color = Color.black;
    //                 }
    //             Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
    //         }
    //     }
    // }


}