using UnityEngine;

/// <summary>
/// Class that guides navigation through the campaign map
/// Simplifies having to pathfind a bunch of stuff
/// </summary>
public class MapPath : MonoBehaviour
{
    public bool PathIsUnlocked { get; private set; }
    
    [SerializeField]
    MapNode origin;

    [SerializeField]
    MapNode destination;
    
    [SerializeField] 
    Transform[] pathStops;

}