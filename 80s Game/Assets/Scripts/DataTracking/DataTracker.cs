using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


// Factory class to delegate saving data to correct handler
public static class DataSaveFactory
{
    public static DataSaver MakeSaver(bool remoteAvailable)
    {
        if (remoteAvailable)
        {
            return new RemoteDataSaver();
        }
        return new FileSystemDataSaver();
    }  
}

// Abstract data save class
public abstract class DataSaver
{
    /// <summary>
    /// Save the data item that encapsulates player behavior.
    /// Handled asyncronously through the use of coroutines to minimize
    /// gameplay impact
    /// </summary>
    /// <param name="data">Data item</param>
    /// <returns></returns>
    public abstract IEnumerator Save(SaveDataItem data);
}


// Concrete data save class for interfacing with the local file system
public class FileSystemDataSaver : DataSaver
{
    public override IEnumerator Save(SaveDataItem data)
    {
        // Establish a location where the data will be locally written to
        string filePath = Application.dataPath + "/PlayerData/GameplayStats/";
        string fileName = Guid.NewGuid().ToString() + ".json";
        string dirPath = Path.GetDirectoryName(filePath + fileName);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // Write the data
        File.WriteAllText(filePath + fileName, JsonUtility.ToJson(data));
        yield return null;
    }
}


// Concrete data save class for sending data to a remote destination
public class RemoteDataSaver : DataSaver
{
    string endpoint = "/save";
    public override IEnumerator Save(SaveDataItem data)
    {
        // Prepare the request
        string url = "http://localhost:80" + endpoint;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Content-Type", "application-json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        //Send the request
        yield return request.SendWebRequest();
        
        // Fallback has been tested
        bool requestError = request.result != UnityWebRequest.Result.Success;
        if ( requestError )
        {
            FileSystemDataSaver saver = new FileSystemDataSaver();
            yield return saver.Save(data);
        }
    }
}