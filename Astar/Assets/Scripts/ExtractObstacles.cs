using UnityEngine;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

//Author : Akashleena Sarkar (akashleena.s@newspace.co.in)
public class ExtractObstacles : MonoBehaviour
{
    //public List<Vector3> unwalkableNodes = new List<Vector3>();
     public HashSet<Vector3> unwalkableNodesSet = new HashSet<Vector3>();
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
            //Debug.Log("hello");
            unwalkableNodesSet = g.CreateGrid(sc.polygon1, obstacleid);
           // Debug.Log("OBSTACLE NUMBER = " + obstacleid);
            // for (int i=0; i<unwalkableNodes.Count; i++)
            // {
            //    // Debug.Log("unwalkable nodes = " + unwalkableNodes[i]);
            //     finalobstacleList.Add(unwalkableNodes[i]);

            // }
            foreach(Vector3 obsSet in unwalkableNodesSet)
            {
                finalobstacleList.Add(obsSet);
            }
        }
        
      
    }

    void Update()
    {
  

        if (Input.GetKeyDown(KeyCode.C))
        {
            pf.FindPath(seeker.position, target.position,finalobstacleList);
        }
         if (Input.GetKeyDown(KeyCode.D))
        {

            pf.pathLineRenderer.positionCount = 0;
            pf.finalpathLineRenderer.positionCount = 0;
        }
    }

}

