using UnityEngine;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class ExtractObstacles : MonoBehaviour
{
    public List<Vector3> unwalkableNodes = new List<Vector3>();
    public List<List<Vector3>> finalobstacleList;
    Grids g ;
    Pathfinding pf ;
    [Serializable]
    public class SerializableClass
    {
        public List<Vector3> polygon1 = new List<Vector3>();

    }
    public List<SerializableClass> obstacleList;    
    int obstacleid=0;
    void Awake()
    {
        g = gameObject.GetComponent<Grids>();
        pf = gameObject.GetComponent<Pathfinding>();
       
        finalobstacleList = new List<List<Vector3>>();
        
    }
    void Start()
    {
        ExtractallObstacles();
    }
    public void ExtractallObstacles()
    {
        Debug.Log("Inside extract all obstacles");
        foreach (SerializableClass sc in obstacleList)
        {
            obstacleid++;
            Debug.Log("hello");
            unwalkableNodes = g.CreateGrid(sc.polygon1, obstacleid);
            finalobstacleList.Add(unwalkableNodes);
        }
    pf.SetNodewalkability(finalobstacleList, g.gridSizeX, g.gridSizeY);      
    }
}

