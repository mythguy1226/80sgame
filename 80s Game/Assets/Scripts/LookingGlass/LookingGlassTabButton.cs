using UnityEngine;

/// <summary>
/// Class that governs the buttons for navigating along different tabs in the LookingGlass tool
/// </summary>
public class LookingGlassTabButton : MonoBehaviour
{
    private LookingGlassUI uiManager;

    private void Awake()
    {
        uiManager = GetComponentInParent<LookingGlassUI>();
    }

    /// <summary>
    /// Open a specific panel
    /// </summary>
    /// <param name="panel">The LookingGlass panel to open</param>
    public void OpenPanel(LookingGlassPanel panel)
    {
        uiManager.SetActivePanel(panel);
    }
}