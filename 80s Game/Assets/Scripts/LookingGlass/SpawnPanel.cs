using System.Collections.Generic;
using UnityEngine;
public class SpawnPanel : LookingGlassPanel
{
    [SerializeField]
    private Sprite available;
    [SerializeField]
    private Sprite notAvailable;
    Dictionary<TargetManager.TargetType, SpawnCommandGroup> spawnUIGroups;

    public void Awake()
    {
        spawnUIGroups = new Dictionary<TargetManager.TargetType, SpawnCommandGroup>();
    }

    /// <summary>
    /// Bind a control group to its target type in the dictionary for easy reference
    /// Binds happen only once
    /// </summary>
    /// <param name="type">the type of bat the group belongs to</param>
    /// <param name="group">The group object itself</param>
    public void EnlistToGroup(TargetManager.TargetType type, SpawnCommandGroup group)
    {
        if (!spawnUIGroups.ContainsKey(type))
        {
            spawnUIGroups.Add(type, group);
        }

        if (!GameManager.Instance.ActiveGameMode.CanSpawnType(type))
        {
            spawnUIGroups[type].UpdateIcon(notAvailable);
        }
    }

    /// <summary>
    /// Tell the corresponding group to update its icon with the corresponding one, depending on spawn availability
    /// </summary>
    /// <param name="type">The int that represents the TargetType enum</param>
    public void UpdateAvailabilityUI(int type)
    {
        TargetManager.TargetType targetType = (TargetManager.TargetType)type;
        bool canSpawn = GameManager.Instance.ActiveGameMode.CanSpawnType(targetType);
        if (canSpawn)
        {
            spawnUIGroups[targetType].UpdateIcon(available);
        } else
        {
            spawnUIGroups[targetType].UpdateIcon(notAvailable);
        }
    }
}