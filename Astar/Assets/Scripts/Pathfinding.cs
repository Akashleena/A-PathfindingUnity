
using System.ComponentModel;
using System.Net;
using System.IO;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//@Author: Akashleena Sarkar <akashleena.s@newspace.co.in>
public class Pathfinding : MonoBehaviour
{
   // public Transform seeker, target;
    public float epsilon = 150f; //for RDP tolerance
    public float epsilonMax, epsilonMin, epsilonCurrent;
    public Grids myGrid;
    public GameDataManager gdm;
    public Grids g1;
    Reducewaypoints rw = new Reducewaypoints();
    int noofwaypoints;
    public LineRenderer pathLineRenderer = new LineRenderer();
    public LineRenderer finalpathLineRenderer = new LineRenderer();
    public LineRenderer rdppathLineRenderer = new LineRenderer();
    public List<Vector3> waypoints;
    public List<Vector3> reducedwaypoints;
    public HashSet<Vector3> gcsWaypoints;
    public List<Vector3> gcsList;
    public Vector3[] points;
    ExtractObstacles eo;
    public Vector3 start, end;
    public Transform testPrefab;
    public Node[,] newNode;
    void Start()
    {
          myGrid = GetComponent<Grids>();
          g1 = GetComponent<Grids>();
          eo= GetComponent<ExtractObstacles>();
          gcsWaypoints = new HashSet<Vector3>();
          gcsList = new List<Vector3>();
          
    }
    public Grids SetNodewalkability(List<Vector3> finalobstacleList, int gridSizeX, int gridSizeY)
    {
        // Debug.Log("final obstacle list count " + finalobstacleList.Count);            

        for (int x = 0; x < myGrid.gridSizeX; x++)
        {
            for (int y = 0; y < myGrid.gridSizeY; y++)
           {         
                Vector3 worldPoint = myGrid.worldBottomLeft + Vector3.right * (x * myGrid.nodeDiameter + myGrid.nodeRadius) + Vector3.forward * (y * myGrid.nodeDiameter + myGrid.nodeRadius);                      
                        
                myGrid.grid[x, y] = new Node(true, worldPoint, x, y);                         
                            
                        
            }
        }

           
              
       for (int x = 0; x < myGrid.gridSizeX; x++)
        {
            for (int y = 0; y < myGrid.gridSizeY; y++)
           {         
                Vector3 worldPoint = myGrid.worldBottomLeft + Vector3.right * (x * myGrid.nodeDiameter + myGrid.nodeRadius) + Vector3.forward * (y * myGrid.nodeDiameter + myGrid.nodeRadius);
                         
                for (int i = 0; i < finalobstacleList.Count; i++)
                { 
                    //Vector3 worldPoint = finalobstacleList[i];
                    //
                    if (worldPoint.x == finalobstacleList[i].x && worldPoint.y == finalobstacleList[i].y && worldPoint.z== finalobstacleList[i].z)
                    {
                        //myGrid.grid[x, y] = new Node(false, worldPoint, x, y);
                        myGrid.grid[x, y].walkable = false;
                    // finalGrid=mygrid;
                        // Vector3 objectPOS5 = worldPoint;
                        // var obstacleprefab = Instantiate(testPrefab, objectPOS5, Quaternion.identity);
                        // obstacleprefab.GetComponent<Renderer>().material.color = Color.red;
                                                                    
                    }                          
                    
                }
            }
        }

        int k=0;
          for (int i = 0; i < myGrid.gridSizeX; i++)
        {
            for (int j = 0; j < myGrid.gridSizeY; j++)
            {
                if (myGrid.grid[i,j].walkable==false)
                {
                   // Debug.Log(myGrid.grid[i,j].worldPosition);
                k++;
                }
            }
        }
               
        Debug.Log("Total non walkable nodes = " + k);  

          int l=0;
          for (int i = 0; i < myGrid.gridSizeX; i++)
        {
            for (int j = 0; j < myGrid.gridSizeY; j++)
            {
                if (myGrid.grid[i,j].walkable==true)
                {
                l++;
                }
            }
        }
            
        Debug.Log("Total walkable nodes = " + l);  
        return myGrid;  
    }

    void GizmosBypass(List<Node> path)
    {
        int i = 0;
        int noofwaypoints = path.Count;
        points = new Vector3[path.Count];
        waypoints = new List<Vector3>();

        // points[path.Count - 1] = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        points[path.Count - 1] = new Vector3(end.x, end.y, end.z);
        pathLineRenderer.positionCount = noofwaypoints;
        foreach (Node n in path)
        {
            points[i] = myGrid.worldBottomLeft + Vector3.right * (n.gridX * myGrid.nodeDiameter + myGrid.nodeRadius) + Vector3.forward * (n.gridY * myGrid.nodeDiameter + myGrid.nodeRadius);
            waypoints.Add(points[i]);
          pathLineRenderer.SetPosition(i, points[i]);
            i++;
        }


        reducedwaypoints = rw.DouglasPeuckerReduction(waypoints, epsilon);
        // pathLineRenderer.positionCount = reducedwaypoints.Count;
        //  for (int j = 0; j < reducedwaypoints.Count; j++)
        // {
        //     pathLineRenderer.SetPosition(j, reducedwaypoints[j]);
        // }
        Vector3 endWaypoint = reducedwaypoints[(reducedwaypoints.Count-1)];
       RdpHeuristic(reducedwaypoints, waypoints, endWaypoint);

       
        
       
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos, List<Vector3> finalobstacleList)
    {
        Debug.Log("startPos" + startPos);
        Debug.Log("targetPos" + targetPos);
        int noofwalkable =0;
        start = startPos;
        end = targetPos;
        myGrid= SetNodewalkability(finalobstacleList, myGrid.gridSizeX, myGrid.gridSizeY);
        Node startNode = myGrid.NodeFromWorldPoint(startPos);
        Node targetNode = myGrid.NodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
       
         for (int x = 0; x < myGrid.gridSizeX; x++)
        {
            for (int y = 0; y < myGrid.gridSizeY; y++)
            {
                if (myGrid.grid[x,y].walkable == false)
                noofwalkable +=1;
            }
        }
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
                   RetracePath(startNode, targetNode, myGrid);

                return;
            }
           

            foreach (Node neighbour in myGrid.GetNeighbours(node))
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

    void RetracePath(Node startNode, Node endNode, Grids myGrid)
    {

        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        myGrid.path = path;
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
   


     public void RdpHeuristic(List<Vector3> rdpWaypoints, List<Vector3> astarWaypoints, Vector3 endWaypoint) 
    {
        Debug.Log("END WAYPOINT" + endWaypoint);
       int startIndex = 0, endIndex=2, copyofendIndex=2;
       gcsWaypoints.Add(astarWaypoints[startIndex]);
       while (endIndex<=(astarWaypoints.Count-1))
        {
            Debug.Log("astarWaypoints[endIndex]" + astarWaypoints[endIndex]);
             int nooftimeslineIntersects=0;
            foreach (var sc in eo.obstacleList)
            {
                bool lineIntersects= g1.checkLineinsidepolygon(sc.polygon1, sc.polygon1.Count, astarWaypoints[startIndex], astarWaypoints[endIndex]);
                if (lineIntersects)
                {
                    nooftimeslineIntersects= nooftimeslineIntersects+1;
                }
            }
            Debug.Log("no of time line intersects"+ nooftimeslineIntersects+ " for start and end as "+ astarWaypoints[startIndex] + astarWaypoints[endIndex]);
           
            if (nooftimeslineIntersects==0)
            {
                 copyofendIndex= endIndex;
            
                
            }
            else
            {
                gcsWaypoints.Add(astarWaypoints[copyofendIndex]);
                startIndex=copyofendIndex;   
                Debug.Log("new start node" + astarWaypoints[startIndex]); 
            }
            endIndex= endIndex+1;
        }
            gcsWaypoints.Add(astarWaypoints[(astarWaypoints.Count-1)]);
            gcsList = gcsWaypoints.ToList();
            Debug.Log(gcsList.Count);
           

            // gcsList.Add(gcsWaypoints[i]);
         finalpathLineRenderer.positionCount = gcsList.Count;
         for (int j = 0; j < gcsList.Count; j++)
        {
            finalpathLineRenderer.SetPosition(j, gcsList[j]);
        }
         gdm.writeFile(gcsList);
    }
}
