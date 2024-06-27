using UnityEngine;
public static class NetworkUtility
{
    // Currently pointing to localhost until we can set this up better
    public static string destinationURL = "https://35v53w7wwj.execute-api.us-east-1.amazonaws.com/default/SaveToMongo";

    /// <summary>
    /// Function that checks whether or not this environment should be sending data remotely
    /// Development environments do not send data to remote
    /// </summary>
    /// <returns>Returns true if this is a development environment</returns>
    public static bool NetworkDevEnv()
    {
        bool debug = Debug.isDebugBuild;
#if UNITY_EDITOR
        debug = true;
#endif
        return debug;
    }
}