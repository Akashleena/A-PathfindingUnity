using UnityEngine;
using System.Collections.Generic;

public class Node
{

    public bool walkable;
    public bool visited;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;

    public Node()
    {
        walkable = true;
        worldPosition = new Vector3(0, 0, 0);
        gridX = 0;
        gridY = 0;
        visited = false;
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, bool _visited)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        visited = _visited;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
