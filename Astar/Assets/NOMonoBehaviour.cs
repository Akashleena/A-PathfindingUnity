using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOMonoBehaviour : MonoBehaviour
{
    public List<GameObject> GGObstacles;

    List<Node> NodesofGG;

    private void Awake()
    {
        foreach (var obj in GGObstacles)
        {
            NodesofGG.Add(obj.GetComponent<Node>());
        }
    }   
}
