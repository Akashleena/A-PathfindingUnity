using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InputData 
{
    public Vector3 startPos;
    public Vector3 endPos;
}

[System.Serializable]
public class OutputData
{
    public List<Vector3> wayPoints;
}

public class GameData : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;

    public List<Vector3> wayPoints;

    public void LoadInputData(InputData inputData)
    {
        this.startPos = inputData.startPos;
        this.endPos = inputData.endPos;
    }

    public void LoadOutputData(OutputData outputData, List<Vector3> points)
    {
        outputData.wayPoints = this.wayPoints = points;
    }
    public Vector3 returnstartPos()
    {
        return this.startPos;
    }
       public Vector3 returnendPos()
    {
        return this.endPos;
    }
}