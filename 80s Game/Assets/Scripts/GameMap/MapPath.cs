using UnityEngine;

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