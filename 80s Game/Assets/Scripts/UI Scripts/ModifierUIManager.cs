using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierUIManager : MonoBehaviour
{
    public List<GameObject> modifierContainers;

    public static ModifierUIManager Instance { get; private set; }

    public void Awake()
    {
        // Check if the static reference matches the script instance
        if(Instance != null && Instance != this)
        {
            // If not, then the script is a duplicate and can delete itself
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject CreateModifierUI(GameObject uiPrefab, int player)
    {
        return Instantiate(uiPrefab, modifierContainers[player].transform);
    }
}
