using UnityEngine;

/// <summary>
/// Points of interest on the campaign map
/// </summary>
public class MapNode : MonoBehaviour
{
    [SerializeField]
    MapPath[] outPaths;

    [SerializeField]
    LevelDataSO levelData;
}