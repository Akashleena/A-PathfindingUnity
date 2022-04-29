using UnityEngine;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class ExtractObstacles : MonoBehaviour
{
    public List<Vector3> unwalkableNodes = new List<Vector3>();
    public List<Vector3> finalobstacleList ;
    Grids g ;
    public Transform seeker, target;
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
       
        finalobstacleList = new List<Vector3>();
        
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
            for (int i=0; i<unwalkableNodes.Count; i++)
            {
                finalobstacleList.Add(unwalkableNodes[i]);
            }
        }
     pf.FindPath(seeker.position, target.position,g,finalobstacleList);     
    }
}

