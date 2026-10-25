using System.IO;
using UnityEngine;
public abstract class GameFileSaver
{
    public abstract void Save(int fileIndex, GameFileData file);
    public abstract GameFileData LoadFile(int index);
}

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

public class SteamFileSaver : GameFileSaver
{
    public override GameFileData LoadFile(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Save(int fileIndex, GameFileData file)
    {
        throw new System.NotImplementedException();
    }
}