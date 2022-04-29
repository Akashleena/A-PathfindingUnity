
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
    public Grids astarGrid, newGrid;
    Reducewaypoints rw = new Reducewaypoints();
    int noofwaypoints;
    public LineRenderer pathLineRenderer = new LineRenderer();
    public LineRenderer finalpathLineRenderer = new LineRenderer();
    public List<Vector3> waypoints;
    public List<Vector3> reducedwaypoints;
    public Vector3[] points;
    public Transform testPrefab;
    public Node[,] newNode;
    void Start()
    {
          astarGrid = GetComponent<Grids>();
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

    public Grids SetNodewalkability(List<Vector3> finalobstacleList, int gridSizeX, int gridSizeY, Grids mygrid, Grids finalGrid )
    {
         Debug.Log("obstacle nodes count " + finalobstacleList.Count);            

       // newGrid = mygrid;
      //  newGrid.grid = new Node[astarGrid.gridSizeX, astarGrid.gridSizeY];
         
            // for (int i = 0; i < astarGrid.gridSizeX; i++)
            //     for (int j = 0; j < astarGrid.gridSizeY; j++)
            //         newGrid.grid[i, j] = new Node();
        
              
        for (int x = 0; x < astarGrid.gridSizeX; x++)
        {
            for (int y = 0; y < astarGrid.gridSizeY; y++)
            {         
                Vector3 worldPoint = mygrid.worldBottomLeft + Vector3.right * (x * mygrid.nodeDiameter + mygrid.nodeRadius) + Vector3.forward * (y * mygrid.nodeDiameter + mygrid.nodeRadius);
                        for (int i = 0; i < finalobstacleList.Count; i++)
                        { 
                            if (worldPoint.x == finalobstacleList[i].x && worldPoint.y == finalobstacleList[i].y && worldPoint.z== finalobstacleList[i].z)
                            {
                                mygrid.grid[x, y] = new Node(false, worldPoint, x, y);
                               // finalGrid=mygrid;
                                Vector3 objectPOS5 = worldPoint;
                                var obstacleprefab = Instantiate(testPrefab, objectPOS5, Quaternion.identity);
                                obstacleprefab.GetComponent<Renderer>().material.color = Color.red;
                            }
                            else
                            {
                                mygrid.grid[x, y] = new Node(true, worldPoint, x, y);
                                //finalGrid=mygrid;
                            }
                            finalGrid = mygrid;
                        }
            }
        }

        int k=0;
          for (int i = 0; i < mygrid.gridSizeX; i++)
        {
            for (int j = 0; j < mygrid.gridSizeY; j++)
            {
                if (finalGrid.grid[i,j].walkable==false)
                {
                k++;
                }
            }
        }
        
        Debug.Log("non walkable finalgrid" + k);  
        return finalGrid;  
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
            points[i] = astarGrid.worldBottomLeft + Vector3.right * (n.gridX * astarGrid.nodeDiameter + astarGrid.nodeRadius) + Vector3.forward * (n.gridY * astarGrid.nodeDiameter + astarGrid.nodeRadius);
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
       
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos, Grids mygrid, List<Vector3> finalobstacleList)
    {
        int noofwalkable =0;
        Debug.Log("Inside find path function");
        Grids finalGrid = new Grids();
        finalGrid= SetNodewalkability(finalobstacleList, astarGrid.gridSizeX, astarGrid.gridSizeY, mygrid, finalGrid);
        Node startNode = astarGrid.NodeFromWorldPoint(startPos);
        Node targetNode = astarGrid.NodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        Debug.Log("mygrid gridsize is" + newGrid.gridSizeX);
         for (int x = 0; x < astarGrid.gridSizeX; x++)
        {
            for (int y = 0; y < astarGrid.gridSizeY; y++)
            {
                if (finalGrid.grid[x,y].walkable == false)
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
                   RetracePath(startNode, targetNode, finalGrid);

                return;
            }
           

            foreach (Node neighbour in finalGrid.GetNeighbours(node))
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

    void RetracePath(Node startNode, Node endNode, Grids finalGrid)
    {

        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        finalGrid.path = path;
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
