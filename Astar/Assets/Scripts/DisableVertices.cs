using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>
/// disable the grid corresponding to the vertex points of the polygon
/// <para>
/// <summary>

public class DisableVertices : MonoBehaviour
{
    public Transform cornerPrefab;
    public List<Vector3> DisablePolygonVertex(List<Vector3>polygon1, List<Vector3> unwalkableNodes)
    {

        for (int i = 0; i < polygon1.Count; i++)
        {
         unwalkableNodes.Add(polygon1[i]);
         Vector3 objectPOS1 = polygon1[i];
         var obstacleprefab = Instantiate(cornerPrefab, objectPOS1, Quaternion.identity);
         obstacleprefab.GetComponent<Renderer>().material.color = Color.blue;
      
        }
     
        return unwalkableNodes;
    }
}