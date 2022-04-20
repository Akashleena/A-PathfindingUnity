using UnityEngine;
public  class BoundingRectangle : MonoBehaviour
{
   
    
    /// <summary>
    /// <para>
    /// create a bounding rectangle around the polygon obstacle with sides ranging from minX-2, maxX+2, minZ-2, maxZ+2
    /// </para>
    /// </summary>

   
    public void CreateBoundingRectangle(Grids.SerializableClass sc, LineRenderer obstacleRenderer)
    {

        for (int i = 0; i < sc.polygon1.Count; i++)
        {
           
            if (sc.minX > sc.polygon1[i].x)
                sc.minX = sc.polygon1[i].x;
            if (sc.minZ > sc.polygon1[i].z)
                sc.minZ = sc.polygon1[i].z;
            if (sc.maxX < sc.polygon1[i].x)
                sc.maxX = sc.polygon1[i].x;
            if (sc.maxZ < sc.polygon1[i].z)
                sc.maxZ = sc.polygon1[i].z;
        }
       

        
        obstacleRenderer.positionCount = sc.polygon1.Count;
        for (int i = 0; i < sc.polygon1.Count; i++)
        {
            obstacleRenderer.SetPosition(i, sc.polygon1[i]);
        }

    }
}