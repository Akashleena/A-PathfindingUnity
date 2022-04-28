
using System.ComponentModel;
using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    public float epsilon = 110f; //for RDP tolerance
    public Grids grid;
    Reducewaypoints rw = new Reducewaypoints();
    int noofwaypoints;
    public LineRenderer pathLineRenderer = new LineRenderer();
    public LineRenderer finalpathLineRenderer = new LineRenderer();
    public List<Vector3> waypoints;
    public List<Vector3> reducedwaypoints;
    public Vector3[] points;
    public Transform testPrefab;

    void Start()
    {
      //  eo.ExtractallObstacles();
        grid = GetComponent<Grids>();
        
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {

    //         pathLineRenderer.positionCount = 0;
    //         finalpathLineRenderer.positionCount = 0;
    //     }


    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         FindPath(seeker.position, target.position);
    //     }
    // }

    public void SetNodewalkability(List<List<Vector3>> finalobstacleList, int gridSizeX, int gridSizeY)
    {
        Debug.Log("Inside Set node walkability");
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Debug.Log("grid.gridSizeX" + grid.gridSizeX);
                Vector3 worldPoint = grid.worldBottomLeft + Vector3.right * (x * grid.nodeDiameter + grid.nodeRadius) + Vector3.forward * (y * grid.nodeDiameter + grid.nodeRadius);
                foreach (var fol in finalobstacleList)
                {
                        for (int i = 0; i < fol.Count; i++)
                        { 
                            if (worldPoint.x == fol[i].x && worldPoint.y == fol[i].y && worldPoint.z== fol[i].z)
                            {
                                //n.walkable = false;
                                grid.grid[x, y] = new Node(false, worldPoint, x, y);
                                Vector3 objectPOS5 = worldPoint;
                                var obstacleprefab = Instantiate(testPrefab, objectPOS5, Quaternion.identity);
                                obstacleprefab.GetComponent<Renderer>().material.color = Color.red;
                            }
                            else
                            {
                                //n.walkable = true;
                                grid.grid[x, y] = new Node(true, worldPoint, x, y);
                            }
                        }

                }
            }
        }
        FindPath(seeker.position, target.position, ref grid);
    }

    void GizmosBypass(List<Node> path)
    {
        int i = 0;
        int noofwaypoints = path.Count;
        points = new Vector3[path.Count];
        waypoints = new List<Vector3>();

        points[path.Count - 1] = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        pathLineRenderer.positionCount = noofwaypoints;
        foreach (Node n in path)
        {
            points[i] = grid.worldBottomLeft + Vector3.right * (n.gridX * grid.nodeDiameter + grid.nodeRadius) + Vector3.forward * (n.gridY * grid.nodeDiameter + grid.nodeRadius);
            waypoints.Add(points[i]);
            pathLineRenderer.SetPosition(i, points[i]);
            i++;
        }

        reducedwaypoints = rw.DouglasPeuckerReduction(waypoints, epsilon);
        finalpathLineRenderer.positionCount = reducedwaypoints.Count;
        for (int j = 0; j < reducedwaypoints.Count; j++)
        {
            finalpathLineRenderer.SetPosition(j, reducedwaypoints[j]);
        }
        // push points array to line renderer
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos,  ref Grids mygrid)
    {
        int noofwalkable =0;
        Debug.Log("Inside find path function");
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        Debug.Log("mygrid gridsize is" + mygrid.gridSizeX);
         for (int x = 0; x < mygrid.gridSizeX; x++)
        {
            for (int y = 0; y < mygrid.gridSizeY; y++)
            {
                if (mygrid.grid[x,y].walkable == false)
                noofwalkable +=1;
            }
        }
        Debug.Log("Number of walkable " + noofwalkable);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                // Debug.Log(startNode.worldPosition + " " + targetNode.worldPosition);
                Debug.Log("calling retarce path");
                RetracePath(startNode, targetNode, ref mygrid);

                return;
            }
           

            foreach (Node neighbour in mygrid.GetNeighbours(node))
            {
                Debug.Log("trying to get neighbours");
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    Debug.Log("calculating nearest neighbours");
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode, ref Grids mygrid)
    {

        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        mygrid.path = path;
        Debug.Log("Calling GizmosBypass");
        GizmosBypass(path);

    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
