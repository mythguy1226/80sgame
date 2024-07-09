using System.Collections.Generic;
using UnityEngine;
using static TargetManager;
public class ModPanel : LookingGlassPanel
{
    [SerializeField]
    private Sprite available;
    [SerializeField]
    private Sprite notAvailable;
    Dictionary<AbsModifierEffect.ModType, ModCommandGroup> modifierUIGroups;

    public void Awake()
    {
        modifierUIGroups = new Dictionary<AbsModifierEffect.ModType, ModCommandGroup>();
    }

    /// <summary>
    /// Bind a control group to its target type in the dictionary for easy reference
    /// Binds happen only once
    /// </summary>
    /// <param name="type">the type of bat the group belongs to</param>
    /// <param name="group">The group object itself</param>
    public void EnlistToGroup(AbsModifierEffect.ModType type, ModCommandGroup group)
    {
        if (!modifierUIGroups.ContainsKey(type))
        {
            modifierUIGroups.Add(type, group);
        }

        if (!GameManager.Instance.ActiveGameMode.isModifierAllowed(type))
        {
            modifierUIGroups[type].UpdateIcon(notAvailable);
        }
    }

    /// <summary>
    /// Tell the corresponding group to update its icon with the corresponding one, depending on spawn availability
    /// </summary>
    /// <param name="type">The int that represents the TargetType enum</param>
    public void UpdateAvailabilityUI(int type)
    {
        AbsModifierEffect.ModType targetType = (AbsModifierEffect.ModType)type;
        bool canSpawn = GameManager.Instance.ActiveGameMode.isModifierAllowed(targetType);
        if (canSpawn)
        {
            modifierUIGroups[targetType].UpdateIcon(available);
        } else
        {
            modifierUIGroups[targetType].UpdateIcon(notAvailable);
        }
    }
}