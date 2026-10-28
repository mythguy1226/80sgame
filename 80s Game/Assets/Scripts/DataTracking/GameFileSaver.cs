using System.IO;
using UnityEngine;

/// <summary>
/// Abstract class that governs saving and loading campaign game files
/// </summary>
public abstract class GameFileSaver
{
    /// <summary>
    /// Saves the game file corresponding to its particular index
    /// </summary>
    /// <param name="fileIndex">The index of the file</param>
    /// <param name="file">The actual game data to save</param>
    public abstract void Save(int fileIndex, GameFileData file);

    /// <summary>
    /// Loads the data corresponding to a campaign game file
    /// </summary>
    /// <param name="index">Which file slot to load</param>
    /// <returns>The data for a game file</returns>
    public abstract GameFileData LoadFile(int index);
}

/// <summary>
/// Class that saves the campaign files to the local machine file system as JSON objects
/// 
/// We might want to use another form of serialization, but for now this works
/// </summary>
public class IOFileSaver : GameFileSaver
{
    private string filePath = Application.dataPath + "/saveFiles/";
    public override GameFileData LoadFile(int index)
    {
        string loadFilePath = $"savefile-{index.ToString()}.json";
        if (!File.Exists(filePath + loadFilePath))
        {
            GameFileData emptyFileData = new GameFileData();
            return emptyFileData;
        }
        string output = File.ReadAllText(filePath + loadFilePath);
        GameFileData readData = JsonUtility.FromJson<GameFileData>(output);
        if (readData == null)
        {
            readData = new GameFileData();
        }
        return readData;
    }

    public override void Save(int fileIndex, GameFileData file)
    {
        string jsonString = JsonUtility.ToJson(file);
        Debug.Log(jsonString);
        string saveFilePath = $"savefile-{fileIndex.ToString()}.json";

        string dirPath = Path.GetDirectoryName(filePath);
        // Create the file if it doesn't exist
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllText(filePath + saveFilePath, jsonString);

    }
}

/// <summary>
/// Wrapper class for the IOFileSaver that also saves stuff to the Steam Cloud
/// </summary>
public class SteamFileSaver : GameFileSaver
{
    IOFileSaver ioFileSaver;

    public SteamFileSaver()
    {
        ioFileSaver = new IOFileSaver();
    }


    public override GameFileData LoadFile(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Save(int fileIndex, GameFileData file)
    {
        // Save locally first
        ioFileSaver.Save(fileIndex, file);

        // Do Steam cloud save stuff
        // To do: add steam cloud save stuff
    }
}