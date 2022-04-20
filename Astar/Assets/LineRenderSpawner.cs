using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderSpawner : MonoBehaviour
{

    public GameObject prefab;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            var ob = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
