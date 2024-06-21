using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Handled asyncronously through the use of coroutines to minimize gameplay impact
    /// </summary>
    /// <param name="data">Data item</param>
    /// <returns></returns>
    public abstract IEnumerator Save(SaveDataItem data);
}


// Concrete data save class for interfacing with the local file system
public class FileSystemDataSaver : DataSaver
{
    string filePath;
    int fileLimitCount;
    public FileSystemDataSaver()
    {
        filePath = Application.dataPath + "/PlayerData/GameplayStats/";
        fileLimitCount = 2;
    }
    public override IEnumerator Save(SaveDataItem data)
    {
        // Establish a location where the data will be locally written to
        string fileName = Guid.NewGuid().ToString() + ".json";
        string dirPath = Path.GetDirectoryName(filePath + fileName);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // Write the data
        File.WriteAllText(filePath + fileName, JsonUtility.ToJson(data));
        EliminateExcessFiles();
        yield return null;
    }


    /// <summary>
    /// List all stored files of JSON game data
    /// </summary>
    /// <returns>A list of FileInfo objects</returns>
    public List<FileInfo> GetDataFiles()
    {
        List<FileInfo> files = new DirectoryInfo(filePath).GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).ToList();
        return files;
    }


    /// <summary>
    /// Eliminate all stored data files that exceed maximum limit amounts and their 
    /// associated meta files if they exist
    /// </summary>
    public void EliminateExcessFiles()
    {
        List<FileInfo> files = GetDataFiles();
        for (int i = fileLimitCount + 1; i < files.Count; i++)
        {
            File.Delete(files[i].FullName);
            if (File.Exists(files[i].FullName + ".meta"))
            {
                File.Delete(files[i].FullName + ".meta");
            }   
        }
    }

    /// <summary>
    /// Eliminate file in data store and it's meta store, should one exist
    /// </summary>
    public void DeleteFile(string fileName)
    {
        File.Delete(fileName);
        if (File.Exists(fileName + ".meta"))
        {
            File.Delete(fileName + ".meta");
        }
    }

    /// <summary>
    /// Read the contents of a file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>The string contents of a file</returns>
    public string GetFileContents(string filePath)
    {
        return File.ReadAllText(filePath);
    } 
}


// Concrete data save class for sending data to a remote destination
public class RemoteDataSaver : DataSaver
{
    public override IEnumerator Save(SaveDataItem data)
    {
        //Save files locally if this is a development environment
        if (NetworkUtility.NetworkDevEnv())
        {
            FileSystemDataSaver saver = new FileSystemDataSaver();
            yield return saver.Save(data);
            yield break;
        }

        // Prepare the request
        string url = NetworkUtility.destinationURL;
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        //Send the request
        yield return request.SendWebRequest();
        
        // Fallback method in case our server becomes unreachable or the request fails
        bool requestError = request.result != UnityWebRequest.Result.Success;
        if ( requestError )
        {
            FileSystemDataSaver saver = new FileSystemDataSaver();
            yield return saver.Save(data);
        }
    }

    /// <summary>
    /// Syncs files to remote
    /// </summary>
    /// <param name="file">Index of the file to sync</param>
    /// <param name="data">String of JSON-compliant data to send to the server</param>
    /// <param name="callback">The function to call when the sync process is complete</param>
    /// <returns>Nothing, this is a couroutine</returns>
    public IEnumerator Sync(int file, string data, Action<int, bool> callback)
    {
        // Prepare the request
        string url = NetworkUtility.destinationURL;
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        //Send the request
        yield return request.SendWebRequest();
        
        // Fallback method in case our server becomes unreachable or the request fails
        bool requestError = request.result != UnityWebRequest.Result.Success;
        callback.Invoke(file, requestError);
    }
}