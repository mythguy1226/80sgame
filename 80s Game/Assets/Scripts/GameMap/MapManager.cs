using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    bool showingUI;
    Dictionary<(int,int), MapNode> nodes;
    public static MapManager Instance { get; private set; }
    MapNode activeNode;


    private void Awake()
    {
        nodes = new Dictionary<(int, int), MapNode>();
        if (Instance != null && Instance != this)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        PlayerAvatar.submitReceived += HandleSubmit;
    }

    private void OnDisable()
    {
        PlayerAvatar.submitReceived -= HandleSubmit;
    }

    private void OnDestroy()
    {
        PlayerAvatar.submitReceived -= HandleSubmit;
    }

    public void RegisterNode(int x, int y, MapNode node)
    {
        nodes.Add((x,y), node);
    }

    private void HandleSubmit()
    {
        if (showingUI)
        {
            
        }
    }
    


}