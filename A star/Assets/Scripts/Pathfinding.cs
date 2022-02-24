using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{

    public Transform seeker, target;
    Grid grid;

    int noofwaypoints;
    public LineRenderer pathLineRenderer = new LineRenderer();

    public Vector3[] points;

    void Awake()
    {
        grid = GetComponent<Grid>();
        //pathLineRenderer = new LineRenderer();
    }

    void Start()
    {
        FindPath(seeker.position, target.position);
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    async void GizmosBypass(List<Node> path)
    {
        // if (!Input.GetKeyDown(KeyCode.A)) return;

        int i = 0;
        int noofwaypoints = path.Count;
        points = new Vector3[path.Count];
        pathLineRenderer.positionCount = points.Length;
        points[path.Count - 1] = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        foreach (Node n in path)
        {
            points[i] = grid.worldBottomLeft + Vector3.right * (n.gridX * grid.nodeDiameter + grid.nodeRadius) + Vector3.forward * (n.gridY * grid.nodeDiameter + grid.nodeRadius);
            // points[i] = new Vector3(n.gridX + grid.worldBottomLeft.x, 0, n.gridY + grid.worldBottomLeft.y);
            pathLineRenderer.SetPosition(i, points[i]);
            i++;
            //add grid location(+bottom_left_x_world and bottom_left_y_world) of n to points


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
