using System.Threading;
using System.ComponentModel;
using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{

    public Transform seeker, target;

    public float epsilon = 1000f; //for RDP tolerance
    Grid grid;
    GrahamScan gs;
    Reducewaypoints rw = new Reducewaypoints();
    int noofwaypoints;
    public LineRenderer pathLineRenderer = new LineRenderer();
    public LineRenderer finalpathLineRenderer = new LineRenderer();
    public List<Vector3> waypoints;
    public List<Vector3> reducedwaypoints;
    public List<Vector3> obstaclevertex;

    public Vector3[] points;

    void Awake()
    {
        grid = GetComponent<Grid>();
        //pathLineRenderer = new LineRenderer();
    }

    // void Start()
    // {
    //     // ObstacleCoordinates();
    //     FindPath(seeker.position, target.position);

    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            FindPath(seeker.position, target.position);
        }
    }

    void ObstacleCoordinates()
    {

        obstaclevertex.Add(new Vector3(12.5f, 0f, 60f));
        obstaclevertex.Add(new Vector3(40.6f, 0f, 90f));
        obstaclevertex.Add(new Vector3(250f, 0f, 300f));
        obstaclevertex.Add(new Vector3(110f, 0, 200f));
        gs.convexHull(obstaclevertex);
    }

    void GizmosBypass(List<Node> path)
    {
        // if (!Input.GetKeyDown(KeyCode.A)) return;

        int i = 0;
        int noofwaypoints = path.Count;
        points = new Vector3[path.Count];
        waypoints = new List<Vector3>();

        points[path.Count - 1] = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        pathLineRenderer.positionCount = noofwaypoints;
        foreach (Node n in path)
        {
            points[i] = grid.worldBottomLeft + Vector3.right * (n.gridX * grid.nodeDiameter + grid.nodeRadius) + Vector3.forward * (n.gridY * grid.nodeDiameter + grid.nodeRadius);
            // points[i] = new Vector3(n.gridX + grid.worldBottomLeft.x, 0, n.gridY + grid.worldBottomLeft.y);
            waypoints.Add(points[i]);
            pathLineRenderer.SetPosition(i, points[i]);
            i++;

            //add grid location(+bottom_left_x_world and bottom_left_y_world) of n to points


        }
        reducedwaypoints = rw.DouglasPeuckerReduction(waypoints, epsilon);
        finalpathLineRenderer.positionCount = reducedwaypoints.Count;
        for (int j = 0; j < reducedwaypoints.Count; j++)
        {
            finalpathLineRenderer.SetPosition(j, reducedwaypoints[j]);
        }



        // push points array to line renderer
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

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
                Debug.Log(startNode.worldPosition + " " + targetNode.worldPosition);
                RetracePath(startNode, targetNode);

                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
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
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
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
