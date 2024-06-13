
using System;
using System.Collections;

using UnityEngine.Networking;

public static class NetworkUtility
{

    // Currently pointing to localhost until we can set this up better
    public static string destinationURL = "http://localhost:4000/";

    public static string testURL = "http://google.com";

    static UnityWebRequest req;

    /// <summary>
    /// Sends a request through the web to test connectivity to the server
    /// </summary>
    /// <param name="callback">A function that takes a boolean parameter</param>
    /// <returns>A coroutine that will set whether this computer can connect to our server or not</returns>
    public static IEnumerator Ping(Action<bool> callback)
    {
        req = new UnityWebRequest(destinationURL, "GET");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            callback.Invoke(false);
        }
        else
        {
            callback.Invoke(true);
        }
    }
}