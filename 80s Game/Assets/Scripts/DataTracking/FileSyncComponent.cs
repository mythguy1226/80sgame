using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSyncComponent : MonoBehaviour
{
    // Class to sync stored files to the remote data server
    // If for whatever reason players can't sync their data to our server, it should get saved locally
    // Then this class handles sending the local files to the remove server for storage
    FileSystemDataSaver fileSystemInterface;
    RemoteDataSaver remoteSystemInterface;
    Dictionary<int, bool> syncStatus;
    List<FileInfo> filenames;
    private void Start()
    {
        // Escape out of this if we're in the debug environment
        if (NetworkUtility.NetworkDevEnv())
        {
           return;
        }

        fileSystemInterface = new FileSystemDataSaver();
        remoteSystemInterface = new RemoteDataSaver();
        syncStatus = new Dictionary<int, bool>();
        filenames =  fileSystemInterface.GetDataFiles();
        if (filenames.Count > 0)
        {
            SyncFile(0, filenames[0].FullName);
        }
        
    }

    /// <summary>
    /// Updates the dictionary that tracks progress on the status of the sync
    /// Begins the process of deletion once all files have been processed
    /// </summary>
    /// <param name="index">The index of the file being updated</param>
    /// <param name="value">The result of the update function</param>
    private void UpdateSyncStatus(int index, bool bIsError)
    {
        syncStatus.Add(index, bIsError);
        if (index < filenames.Count-1)
        {
            SyncFile(index + 1, filenames[index + 1].FullName);
        } else
        {
            DeleteSyncedFiles();
        }
        
    }


    /// <summary>
    /// Starts the coroutine that will save a file to remote.
    /// Passes the UpdateSyncStatus as a callback to the coroutine to call when it's done
    /// </summary>
    /// <param name="index">The index of the file to sync</param>
    /// <param name="filename">The name of the file to sync</param>
    private void SyncFile(int index, string filename)
    {
        StartCoroutine(remoteSystemInterface.Sync(index, fileSystemInterface.GetFileContents(filename), UpdateSyncStatus));
    }

    /// <summary>
    /// Delete all files that have been successfully synced.
    /// Files that aren't synced remain in the host's machine to try syncing in the future.
    /// These files might be deleted by the filesystem if they become too old and stale
    /// </summary>
    private void DeleteSyncedFiles()
    {
        foreach(KeyValuePair<int, bool> kvp in syncStatus)
        {
            if (!kvp.Value) { fileSystemInterface.DeleteFile(filenames[kvp.Key].FullName); }
        }
    }
}