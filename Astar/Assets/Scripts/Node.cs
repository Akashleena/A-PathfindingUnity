using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public float astarWeight = 1f;

    public Node()
    {
        this.walkable = true;
        this.worldPosition = new Vector3(0, 0, 0);
        this.gridX = 0;
        this.gridY = 0;
       
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public float fCost
    {
        get
        {
            return (gCost + (astarWeight * hCost));
        }
    }
}
