using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModEMP : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.EMP;
    }
    public GameObject particles;

    public override void ActivateEffect()
    {
        Instantiate(particles, transform.position, Quaternion.identity);

        // Finds all active targets (i.e., on-screen targets).
        List<Target> activeTargets = GameManager.Instance.TargetManager.targets.FindAll(target => target.FSM.IsActive());
        bool awkward = true;
        foreach(Target target in activeTargets)
        {
            if (!target.bIsStunned)
            {
                awkward = false; 
                break;
            }
        }
        if (awkward) {
            AchievementManager.UnlockAchievement(AchievementConstants.WELL_THATS_AWKWARD);
        }


        foreach (Target target in GameManager.Instance.TargetManager.ActiveTargets)
        {
            target.SetStunningPlayer(this.activator);
            target.ResolveHit();
        }

        effectDuration = 0.0f;
        HandleModifierCountAchievement();
    }

    public override void DeactivateEffect()
    {
        // throw new System.NotImplementedException();
    }
}
