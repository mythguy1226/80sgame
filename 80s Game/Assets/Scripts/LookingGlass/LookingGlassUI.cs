using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The UI manager for the LookingGlass tool
/// </summary>
public class LookingGlassUI :  MonoBehaviour
{
    private LookingGlassPanel activePanel;
    [SerializeField]
    private GameObject uiParent;
    private bool isVisible;
    public TMP_Dropdown AchievementDropdown;

    private void Awake()
    {
        activePanel = GetComponentInChildren<LookingGlassPanel>(true);
        ShowActivePanel();
        isVisible = false;
        uiParent.gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Toggle the visibility of the LookingGlass tool. The underlying monitoring functions are not affected.
    /// </summary>
    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        uiParent.gameObject.SetActive(isVisible);
    }

    public void Hide()
    {
        isVisible = false;
        uiParent.gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Sets which panel currently is active in the tool. Hides all other panels.
    /// </summary>
    /// <param name="panel">The panel to activate</param>
    public void SetActivePanel(LookingGlassPanel panel)
    {
        activePanel.gameObject.SetActive(false);
        activePanel = panel;
        ShowActivePanel();
    }

    /// <summary>
    /// Show the panel marked as active
    /// </summary>
    public void ShowActivePanel()
    {
        activePanel.gameObject.SetActive(true);
    }

    public void UpdateLookingGlassAchievementInt()
    {
        GetComponent<LookingGlass>().SetAchievementInteger(AchievementDropdown.value);
    }
}