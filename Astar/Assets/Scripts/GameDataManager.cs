// Add System.IO to work with files!
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    //File Path for the InputData
    string inputDataFilePath;

    //File Path for the Output Data
    string outputDataFilePath;

    // Create a GameData field.
    [SerializeField]
    public InputData inputGameData;

    [SerializeField]
    public OutputData outputGameData;

    public GameData gameData;
    public ExtractObstacles extractObs;

    //public List<Vector3> wayPoints;

    void Awake()
    {
        // Update the path once the persistent path exists.
       // saveFile = Application.persistentDataPath + "/gamedata.json";
        inputDataFilePath = Application.dataPath + "/inputData.json";

        outputDataFilePath = Application.dataPath + "/outputData.json";
    }

    public void CopyData()
    {

    }

    public void readFile()
    {
        // Does the file exist?
        if (File.Exists(inputDataFilePath))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(inputDataFilePath);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            inputGameData = JsonUtility.FromJson<InputData>(fileContents);
            extractObs.TakeInput(inputGameData.startPos, inputGameData.endPos);
            gameData.LoadInputData(inputGameData);
        }

        Debug.Log("File Read");
    }

    public void writeFile(List<Vector3> wayPoints)
    {
        //To Create a Sample InputData File
       // inputGameData.startPos = gameData.startPos;
        //inputGameData.endPos = gameData.endPos;
        //string jsonString = JsonUtility.ToJson(inputGameData);
        //File.WriteAllText(inputDataFilePath, jsonString);

        gameData.LoadOutputData(outputGameData, wayPoints);

        string jsonString = JsonUtility.ToJson(outputGameData);

        // Write JSON to file.
        File.WriteAllText(outputDataFilePath, jsonString);

        Debug.Log("File Saved");
    }
}
