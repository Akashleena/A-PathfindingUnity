using UnityEngine;
using System.Collections;

public class DisableNodes : MonoBehaviour
{
    // Use this for initialization
    public CheckifinsideObstacle co;

    public void DisableNodesinsidePolygon(Grids.SerializableClass sc, Vector3 worldPoint, float nodeDiameter)
    {
        //if (worldPoint.x >= (sc.minX - (2.0 * nodeDiameter)) && worldPoint.x <= (sc.maxX + (2.0f * nodeDiameter)) && worldPoint.z >= (sc.minZ - (2 * nodeDiameter)) && worldPoint.z <= (sc.maxZ + (2 * nodeDiameter)))
        //{
        //    #region disabletopandbottomright
        //    if (co.isInside(sc.polygon1, n, bottomLeftPoint, extremeright))
        //    {
        //        walkable = false;

        //        dismantleobstaclenodes.Add(grid[x, y]);
        //        dismantleobstaclenodes.Add(grid[x, y - 1]);
        //        #region Debug statements
        //        //Debug.Log("worldpoint unwalkable right" + worldPoint);
        //        //Debug.Log("bottomnode unwalkable right" + bottomNode);
        //        #endregion


        //    }
        //    #endregion

        //    /// <summary>
        //    /// if the left vector arrow coincides with the polygon once 
        //    /// the point lies inside the polygon obstacle
        //    /// disable the left node and left bottom node
        //    /// </summary>

        //    #region disabletopandbottomleft
        //    if (co.isInside(sc.polygon1, n, bottomLeftPoint, extremeleft))
        //    {
        //        walkable = false;
        //        //grid[x - 1, y] = new Node(walkable, leftNode, x - 1, y, true);
        //        //grid[x - 1, y - 1] = new Node(walkable, bottomLeftNode, x - 1, y - 1, true);
        //        dismantleobstaclenodes.Add(grid[x - 1, y]);
        //        dismantleobstaclenodes.Add(grid[x - 1, y - 1]);
        //        #region Debug statements
        //        //Debug.Log("leftNode unwalkable left vector" + leftNode);
        //        //Debug.Log("bottomLeftNode unwalkable left vector" + bottomLeftNode);
        //        #endregion
        //        //Vector3 objectPOS3 = leftNode;
        //        //var insideObstacleleftnode = Instantiate(testPrefab, objectPOS3, Quaternion.identity);
        //        //insideObstacleleftnode.GetComponent<Renderer>().material.color = Color.red;


        //        Vector3 objectPOS4 = bottomLeftNode;
        //        var insideObstaclebottomleftnode = Instantiate(testPrefab, objectPOS4, Quaternion.identity);
        //        insideObstaclebottomleftnode.GetComponent<Renderer>().material.color = Color.red;

        //    }
        //    #endregion
        //    ///<summary>
        //    /// <remarks>
        //    /// node lies inside the bounding box but it is walkable 
        //    /// </remarks>
        //    /// </summary>
        //    else // lies inside bounding box and it is walkable 
        //    {

        //        CheckWalkableNodesinsideBoundingBox(x, y, worldPoint, "worldpoint");
        //        CheckWalkableNodesinsideBoundingBox(x, (y - 1), bottomNode, "bottomnode");
        //        CheckWalkableNodesinsideBoundingBox((x - 1), y, leftNode, "leftnode");
        //        CheckWalkableNodesinsideBoundingBox((x - 1), (y - 1), bottomLeftNode, "bottomleftnode");
        //    }
        //}
    }
   
}
