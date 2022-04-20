using System.Xml;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class Gridmultipleobstacle : MonoBehaviour
{
    [Header("Grid Parameters")]
    public LayerMask unwalkableMask;
    public bool walkable;
    public Vector2 gridWorldSize;

    public Vector3 bottomLeftPoint, leftNode, bottomNode, bottomLeftNode;
    public float nodeRadius = 50;
    Node[,] grid;
    Node node;
    public int noofobstacles = 1;

    public Vector3 worldBottomLeft;
    // public List<LineRenderer> boundingBox;
    // public GameObject bb;

    // public List<LineRenderer> listofLineRenderers;
    public Transform testPrefab;

    public LineRenderer obstacleRenderer = new LineRenderer();
    public Transform seeker;
    public Vector3 obstaclegridmidpoint;
    [Space]

    [Header("Node Parameters")]
    public float nodeDiameter;
    Vector3 extremeright = new Vector3(INF, 0, 0);
    Vector3 extremeleft = new Vector3(-INF, 0, 0);

    int obstacleGridX, obstacleGridY;
    public float[] perpDist;
    public int gridSizeX, gridSizeY;

    //public Vector3[] polygon1;
    // public Vector3[] boundingRectangle;
    public int n;
    static float INF = 10000.0f;
    //public LineRenderer rightvectorarrow;
    //public LineRenderer leftvectorarrow;
    public CheckifinsideObstacle co;


    [Serializable]
    public class serializableClass
    {
        public List<Vector3> polygon1 = new List<Vector3>();
        public float maxZ = 0.0f, maxX = 0.0f;
        public float minX = 10000f, minZ = 10000f;
        Grids gs;

    }

    [SerializeField]
    public List<serializableClass> obstacleList = new List<serializableClass>();





    void Awake()
    {
        node = new Node(true, Vector3.zero, 0, 0, false);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }
    void Start()
    {

    }

    /// <summary>
    /// <para>
    /// create a bounding rectangle around the polygon obstacle with sides ranging from minX-2, maxX+2, minZ-2, maxZ+2
    /// </para>
    /// </summary>
    void CreateBoundingRectangle(serializableClass sc)
    {
        //boundingRectangle = new Vector3[4];



        for (int i = 0; i < sc.polygon1.Count; i++)
        {
            if (sc.minX > sc.polygon1[i].x)
                sc.minX = sc.polygon1[i].x;
            if (sc.minZ > sc.polygon1[i].z)
                sc.minZ = sc.polygon1[i].z;
            if (sc.maxX < sc.polygon1[i].x)
                sc.maxX = sc.polygon1[i].x;
            if (sc.maxZ < sc.polygon1[i].z)
                sc.maxZ = sc.polygon1[i].z;
        }
        //boundingRectangle[0] = new Vector3(sc.minX - (2 * nodeDiameter), 0f, sc.minZ - (2 * nodeDiameter));
        //boundingRectangle[1] = new Vector3(sc.maxX + (2 * nodeDiameter), 0f, sc.minZ - (2 * nodeDiameter));
        //boundingRectangle[2] = new Vector3(sc.maxX + (2 * nodeDiameter), 0f, sc.maxZ + (2 * nodeDiameter));
        //boundingRectangle[3] = new Vector3(sc.minX - (2 * nodeDiameter), 0f, sc.maxZ + (2 * nodeDiameter));



        //    Debug.Log(k);   
        //bb[k].positionCount = 5;

        //var ob = Instantiate(bb,boundingRectangle[0], Quaternion.identity);
        //var ob1 = Instantiate(bb, boundingRectangle[1], Quaternion.identity);
        //var ob2 = Instantiate(bb, boundingRectangle[2], Quaternion.identity);
        //var ob3 = Instantiate(bb, boundingRectangle[3], Quaternion.identity);

        //listofLineRenderers[k] = ob.GetComponent<LineRenderer>();


        //bb[k].SetPosition(0, boundingRectangle[0]);
        //bb[k].SetPosition(1, boundingRectangle[1]);
        //bb[k].SetPosition(2, boundingRectangle[2]);
        //bb[k].SetPosition(3, boundingRectangle[3]);
        //bb[k].SetPosition(4, boundingRectangle[0]);

        obstacleRenderer.positionCount = sc.polygon1.Count;
        for (int i = 0; i < sc.polygon1.Count; i++)
        {
            obstacleRenderer.SetPosition(i, sc.polygon1[i]);
        }

    }

    void DisablePolygonVertex(serializableClass sc)
    {
        for (int i = 0; i < sc.polygon1.Count; i++)
        {
            //obstacleGridX = (int)((sc.polygon1[i].x + 950) / nodeDiameter); //shift origin to (-950,-950)
            obstacleGridX = (int)((sc.polygon1[i].x - worldBottomLeft.x) / nodeDiameter); //shift origin to (-950,-950)
            obstacleGridY = (int)((sc.polygon1[i].y) / nodeDiameter); //z axis origin need not br shifted
            obstaclegridmidpoint.x = (Mathf.Floor(sc.polygon1[i].x / nodeDiameter) * 100) + nodeRadius;
            obstaclegridmidpoint.z = (Mathf.Floor(sc.polygon1[i].z / nodeDiameter) * 100) + nodeRadius;
            obstaclegridmidpoint.y = 0;
            // Debug.Log("obstaclemidpont" + obstaclegridmidpoint.x + " " + obstaclegridmidpoint.z);

            grid[obstacleGridX, obstacleGridY] = new Node(false, obstaclegridmidpoint, obstacleGridX, obstacleGridY, true);


            //Vector3 objectPOS0 = obstaclegridmidpoint;
            //var obstaclevertices = Instantiate(testPrefab, objectPOS0, Quaternion.identity);
            //obstaclevertices.GetComponent<Renderer>().material.color = Color.black; //remove the line renderers and disable only the vertex coordinates

            // display positions of obstacles vertex and not position of corresponding grid midpoints
            Vector3 objectPOS0 = new Vector3(sc.polygon1[i].x, sc.polygon1[i].y, sc.polygon1[i].z);
            var obstaclevertices = Instantiate(testPrefab, objectPOS0, Quaternion.identity);
            obstaclevertices.GetComponent<Renderer>().material.color = Color.black; //remove the line renderers and disable only the vertex coordinates


        }
    }

    void CheckWalkableNodesinsideBoundingBox(int x, int y, Vector3 worldposition, String outsidebbnode)
    {
        node = grid[x, y];// worldpoint

        if (node.walkable == false) //previously unwalkable
            walkable = false;
        else
            walkable = true;

        if (walkable == false)
        {
            Vector3 objectPOS5 = worldposition;
            var nonwalkableinsidebb = Instantiate(testPrefab, objectPOS5, Quaternion.identity);
            nonwalkableinsidebb.GetComponent<Renderer>().material.color = Color.black;

            //  Debug.Log("else  world point" + worldPoint + "walkable" + walkable);
        }

        grid[x, y] = new Node(walkable, worldposition, x, y, true);
    }

    /// <summary>
    /// <para>
    /// Create 2 vector arrows one towards positive infinity and other towards negative infinity
    /// if the right vector arrow coincides with the polygon once 
    /// the point lies inside the polygon obstacle
    /// disable the right top and right bottom node
    /// </para>
    /// </summary>

    //void VisualizeVectors()
    //{
    //    rightvectorarrow.positionCount = 2;
    //    leftvectorarrow.positionCount = 2;
    //    rightvectorarrow.SetPosition(0, Vector3.zero);      
    //    rightvectorarrow.SetPosition(1, extremeright);
    //    leftvectorarrow.SetPosition(0, Vector3.zero);
    //    leftvectorarrow.SetPosition(1, extremeleft);
    //}
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int i = 0; i < gridSizeX; i++)
            for (int j = 0; j < gridSizeY; j++)
                grid[i, j] = new Node();

        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        bool walkable;

        //  listofLineRenderers = new List<LineRenderer>();

        int k = 0;
        foreach (serializableClass sc in obstacleList)
        {
            Debug.Log("value of sc" + sc);
            CreateBoundingRectangle(sc); //return minX maxX minZ maxZ
            //disable the grid corresponding to the vertex points of the polygon 
            DisablePolygonVertex(sc);
            k++;
            Debug.Log("value of k" + k);
        }

        //we create a grid out of the world map
        // iterate through the grid cells
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //iterate through each obstacle
                Debug.Log("world point" + worldPoint);
                foreach (serializableClass sc in obstacleList)
                {
                    Debug.Log("Inside serializable class for loop");
                    //make only inside the bounding box as unwalkable everything outside bb will be walkable
                    if (worldPoint.x >= (sc.minX - (2.0 * nodeDiameter)) && worldPoint.x <= (sc.maxX + (2.0f * nodeDiameter)) && worldPoint.z >= (sc.minZ - (2 * nodeDiameter)) && worldPoint.z <= (sc.maxZ + (2 * nodeDiameter)))
                    {
                        // VisualizeVectors();
                        #region disabletopandbottomright
                        if (co.isInside(sc.polygon1, n, bottomLeftPoint, extremeright))
                        {
                            walkable = false;
                            grid[x, y] = new Node(walkable, worldPoint, x, y, true);
                            grid[x, y - 1] = new Node(walkable, bottomNode, x, y - 1, true);
                            #region Debug statements
                            Debug.Log("worldpoint unwalkable right" + worldPoint);
                            Debug.Log("bottomnode unwalkable right" + bottomNode);
                            #endregion

                            Vector3 objectPOS1 = worldPoint;
                            var insideObstaclenode = Instantiate(testPrefab, objectPOS1, Quaternion.identity);
                            insideObstaclenode.GetComponent<Renderer>().material.color = Color.red;

                            Vector3 objectPOS2 = bottomNode;
                            var insideBottomnode = Instantiate(testPrefab, objectPOS2, Quaternion.identity);
                            insideBottomnode.GetComponent<Renderer>().material.color = Color.red;
                        }
                        #endregion

                        /// <summary>
                        /// if the left vector arrow coincides with the polygon once 
                        /// the point lies inside the polygon obstacle
                        /// disable the left node and left bottom node
                        /// </summary>

                        #region disabletopandbottomleft
                        if (co.isInside(sc.polygon1, n, bottomLeftPoint, extremeleft))
                        {
                            walkable = false;
                            grid[x - 1, y] = new Node(walkable, leftNode, x - 1, y, true);
                            grid[x - 1, y - 1] = new Node(walkable, bottomLeftNode, x - 1, y - 1, true);
                            #region Debug statements
                            Debug.Log("leftNode unwalkable left vector" + leftNode);
                            Debug.Log("bottomLeftNode unwalkable left vector" + bottomLeftNode);
                            #endregion
                            Vector3 objectPOS3 = leftNode;
                            var insideObstacleleftnode = Instantiate(testPrefab, objectPOS3, Quaternion.identity);
                            insideObstacleleftnode.GetComponent<Renderer>().material.color = Color.red;


                            Vector3 objectPOS4 = bottomLeftNode;
                            var insideObstaclebottomleftnode = Instantiate(testPrefab, objectPOS4, Quaternion.identity);
                            insideObstaclebottomleftnode.GetComponent<Renderer>().material.color = Color.red;

                        }
                        #endregion
                        ///<summary>
                        /// <remarks>
                        /// node lies inside the bounding box but it is walkable 
                        /// </remarks>
                        /// </summary>
                        else // lies inside bounding box and it is walkable 
                        {

                            CheckWalkableNodesinsideBoundingBox(x, y, worldPoint, "worldpoint");
                            CheckWalkableNodesinsideBoundingBox(x, (y - 1), bottomNode, "bottomnode");
                            CheckWalkableNodesinsideBoundingBox((x - 1), y, leftNode, "leftnode");
                            CheckWalkableNodesinsideBoundingBox((x - 1), (y - 1), bottomLeftNode, "bottomleftnode");
                        }
                    }
                }
                //outside bounding box

                walkable = true;
                // Debug.Log("worldpoint outside box" + worldPoint + "is walkable" + walkable);
                grid[x, y] = new Node(walkable, worldPoint, x, y, true);


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