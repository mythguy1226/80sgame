using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to hold behavior for the group of inputs that manage handling spawn debugging
/// </summary>
public class ModCommandGroup: MonoBehaviour
{
    [SerializeField]
    private Image availabilityRenderer;
    public AbsModifierEffect.ModType type;

    private void Start()
    {
        GetComponentInParent<ModPanel>().EnlistToGroup(type, this);
    }

    /// <summary>
    /// Update the UI icon that indicates whether the bat type associated with this group can spawn or not
    /// </summary>
    /// <param name="newIcon">The icon that will replace the existing one</param>
    public void UpdateIcon(Sprite newIcon)
    {
        availabilityRenderer.sprite = newIcon;
    }
}