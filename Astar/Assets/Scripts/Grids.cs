using UnityEngine;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class Grids : MonoBehaviour
{
    #region declare variable
    [Header("Grid Parameters")]
    public Vector2 gridWorldSize;
    int obstacleGridX, obstacleGridY;
    public int gridSizeX, gridSizeY;
   
     public List<Vector3> vertexmidpoints = new List<Vector3>();
    public List<Vector3> vertexmidpoints1 = new List<Vector3>();

    
    public float nodeRadius = 50;
    public float nodeDiameter;
    public Node[,] grid;
    public Node node;
    public Vector3 worldBottomLeft;
    [Space]
    [Header("Obstacle Parameters")]
    public List<float> bounds;
    public LineRenderer obstacleRenderer = new LineRenderer(); 
    public Vector3 obstaclegridmidpoint;
    public Transform cornerPrefab;
    [Space]
   
    [Header("Unwalkable nodes")]
    [Space]
    public List<Vector3> unwalkableNodes = new List<Vector3>();
    public List<Vector3> vertex;
    public List<Vector3> insidevertex ;
    public int n;
    
    [Header("Point lies inside Polygon Algo variables")]
    [Space]
    public Vector3 bottomLeftPoint, leftNode, bottomNode, bottomLeftNode;
    static float INF = 10000.0f;
    public bool walkable;
    private Transform testPrefab;
    Vector3 extremeright = new Vector3(INF, 0, 0);
    Vector3 extremeleft = new Vector3(-INF, 0, 0);
    Vector3 extreme;
    
    #endregion

    void Awake()
    {
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
        grid = new Node[gridSizeX, gridSizeY];
        insidevertex = new List<Vector3>();
        vertex = new List<Vector3>();
        for (int i = 0; i < gridSizeX; i++)
            for (int j = 0; j < gridSizeY; j++)
                grid[i, j] = new Node();
        
            worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            bounds = CreateBoundingRectangle(polygon1, obstacleRenderer, obstacleid);
            Debug.Log("B O U N D S");
            Debug.Log(bounds[0] + " " +bounds[1] + " " +bounds[2] + " " +bounds[3] + " " );

            vertex = DisablePolygonVertex(polygon1, vertex);   
            //  Debug.Log("vertex count" + vertex.Count);
            insidevertex = FindunwalkableNodes(polygon1, insidevertex, obstacleid, bounds);
              Debug.Log("Inside Vertex count" + insidevertex.Count);
            for(int i=0; i<insidevertex.Count; i++)
            {
                unwalkableNodes.Add(insidevertex[i]);
               // Debug.Log("Inside Vertex added" + insidevertex[i]);
                
            }

        

        return unwalkableNodes;

    }

     public List<float> CreateBoundingRectangle(List<Vector3> polygon1, LineRenderer obstacleRenderer, int obstacleid)
    {
        List<float> bounds = new List<float>(4);
        float minX = 10000, minZ = 10000;
        float maxX = -10000, maxZ = -10000;
        for (int i = 0; i < polygon1.Count; i++)
        {
           
            if (minX > polygon1[i].x)
                minX = polygon1[i].x;
            if (minZ > polygon1[i].z)
                minZ = polygon1[i].z;
            if (maxX < polygon1[i].x)
                {
                maxX = polygon1[i].x;
                Debug.Log("maxX" + maxX);
                }
            if (maxZ < polygon1[i].z)
            {
                maxZ = polygon1[i].z;   
                Debug.Log("maxZ" + maxZ);
            }
        }

        bounds.Add(minX);
        bounds.Add(maxX);
        bounds.Add(minZ);
        bounds.Add(maxZ);

        obstacleRenderer.positionCount = polygon1.Count;
        for (int i = 0; i < polygon1.Count; i++)
        {
           obstacleRenderer.SetPosition(i, polygon1[i]);
        }

        return bounds;

    }

    public List<Vector3> DisablePolygonVertex(List<Vector3>polygon1, List<Vector3> vertex)
    {
         for (int i = 0; i < polygon1.Count; i++)
        {
         vertex.Add(polygon1[i]);
         Vector3 objectPOS1 = polygon1[i];
         var obstacleprefab = Instantiate(cornerPrefab, objectPOS1, Quaternion.identity);
         obstacleprefab.GetComponent<Renderer>().material.color = Color.blue;
      
        }
     
        return vertex;
    }

    public List<Vector3> FindunwalkableNodes(List<Vector3> polygon1, List<Vector3> insidevertex, int obstacleid, List<float> bounds)
    {
        
        Debug.Log("Calling find unwalkable");
        Debug.Log("bounds" + bounds[0] +" "+ bounds[1] +" " +bounds[2] +" "+ bounds[3] +" "+  "obstcaleid" + obstacleid);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                int t=0;
                //make only inside the bounding box as unwalkable everything outside bb will be walkable
                if (worldPoint.x >= (bounds[0] - (2.0 * nodeDiameter)) && worldPoint.x <= (bounds[1] + (2.0f * nodeDiameter)) && worldPoint.z >= (bounds[2] - (2 * nodeDiameter)) && worldPoint.z <= (bounds[3] + (2 * nodeDiameter)))
                {
                    calculateFouradjnodes(x, y);
                    // Debug.Log("polygon1" + polygon1.Count);
                    // for (int i=0;i<polygon1.Count;i++)
                    // Debug.Log("polygon1" + polygon1[i]);
                    // Debug.Log("bottomleftpoint" + bottomLeftPoint);
                    // Debug.Log("extremeright" + extremeright);


                    if (checkinsidePolygon(polygon1, polygon1.Count, bottomLeftPoint, extremeright))
                    {
                        //flag++;
                        insidevertex.Add(worldPoint);
                        insidevertex.Add(bottomNode);
                       
                    }

                    if (checkinsidePolygon(polygon1, polygon1.Count, bottomLeftPoint, extremeleft))
                    {
                        
                        insidevertex.Add(leftNode);
                        insidevertex.Add(bottomLeftNode);
                        // Debug.Log("worldposition added" + leftNode);
                        // Debug.Log("worldposition added" + bottomLeftNode);
                    }

                    // else // lies inside bounding box and it is walkable 
                    // {

                    //     CheckWalkableNodesinsideBoundingBox(x, y, worldPoint, insidevertex, "worldpoint");
                    //     CheckWalkableNodesinsideBoundingBox(x, (y - 1), bottomNode, insidevertex, "bottomnode");
                    //     CheckWalkableNodesinsideBoundingBox((x - 1), y, leftNode, insidevertex, "leftnode");
                    //     CheckWalkableNodesinsideBoundingBox((x - 1), (y - 1), bottomLeftNode, insidevertex, "bottomleftnode");
                    // }
                    
                }
            }
        }
            // Debug.Log("flag" + flag + "obstacle number" + obstacleid);
        return insidevertex;
    }

    // private void CheckWalkableNodesinsideBoundingBox(int x, int y, Vector3 worldposition, List<Vector3> insidevertex, String bbnode)
    // {
    //     node = grid[x, y];// worldpoint
    //     if (node.walkable == false) //previously unwalkable
    //     {
    //         walkable = false;
    //         insidevertex.Add(worldposition);
    //     }
    // }

     private void calculateFouradjnodes(int x, int y)
    {
        bottomLeftPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeRadius);//bottom right
        bottomNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter); //subtract node diameter from worldpoint.y
        bottomLeftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius - nodeDiameter);
        leftNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius - nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius);
    }

    static bool onSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Math.Max(p.x, r.x) &&
            q.x >= Math.Min(p.x, r.x) &&
            q.z <= Math.Max(p.z, r.z) &&
            q.z >= Math.Min(p.z, r.z))
        {
            return true;
        }
        return false;
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are collinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    private int orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        // https://www.geeksforgeeks.org/orientation-3-ordered-points/
        float val = (q.z - p.z) * (r.x - q.x) -
                (q.x - p.x) * (r.z - q.z);

        if (val == 0)
        {
            return 0; // collinear
        }
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    // The function that returns true if
    // line segment 'p1q1' and 'p2q2' intersect.
    private bool doIntersect(Vector3 p1, Vector3 q1,
                            Vector3 p2, Vector3 q2)
    {
        // https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        // Find the four orientations needed for
        // general and special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        // Special Cases
        // p1, q1 and p2 are collinear and
        // p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1))
        {
            return true;
        }

        // p1, q1 and p2 are collinear and
        // q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1))
        {
            return true;
        }

        // p2, q2 and p1 are collinear and
        // p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2))
        {
            return true;
        }

        // p2, q2 and q1 are collinear and
        // q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2))
        {
            return true;
        }

        return false;
    }

    // Returns true if the Vector3 p lies
    // inside the polygon[] with n vertices
    public bool checkinsidePolygon(List<Vector3> polygon, int n, Vector3 p, Vector3 extreme)
    {
        
        if (n < 3)
        {
            return false;
        }

           // with sides of polygon
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to
            // 'extreme' intersects with the line
            // segment from 'polygon[i]' to 'polygon[next]'
            if (doIntersect(polygon[i],
                            polygon[next], p, extreme))
            {
                // If the Vector3 'p' is collinear with line
                // segment 'i-next', then check if it lies
                // on segment. If it lies, return true, otherwise false
                if (orientation(polygon[i], p, polygon[next]) == 0)
                {
                    return onSegment(polygon[i], p, polygon[next]);
                }
                count++;
            }
            i = next;
        } while (i != 0);
        return (count % 2 == 1); 
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