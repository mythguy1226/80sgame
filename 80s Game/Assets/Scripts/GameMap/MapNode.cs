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

    [SerializeField]
    int x;

    [SerializeField]
    int y;

    private void OnEnable()
    {
        MapManager.Instance.RegisterNode(x, y, this);
    }

    public LevelDataSO GetLevelData()
    {
        return levelData;
    }
}