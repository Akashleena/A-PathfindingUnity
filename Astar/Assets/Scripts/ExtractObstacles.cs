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
  SetWalkability sw ;
    [Serializable]
    public class SerializableClass
    {
        public List<Vector3> polygon1 = new List<Vector3>();

    }
    public List<SerializableClass> obstacleList;


    int k=0;
    void Awake()
    {
        g = gameObject.GetComponent<Grids>();
        sw = gameObject.GetComponent<SetWalkability>();
        finalobstacleList = new List<List<Vector3>>();
        
    }
    void Start()
    {
        foreach (SerializableClass sc in obstacleList)
        {
            k++;
            Debug.Log("hello");
            unwalkableNodes = g.CreateGrid(sc.polygon1, k);
            finalobstacleList.Add(unwalkableNodes);
        }
        sw.SetNodewalkability(finalobstacleList, g.gridSizeX, g.gridSizeY);
    }
}

