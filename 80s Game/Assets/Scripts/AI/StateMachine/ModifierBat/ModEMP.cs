using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModEMP : AbsModifierEffect
{
    public GameObject particles;

    public override void ActivateEffect()
    {
        Instantiate(particles, transform.position, Quaternion.identity);

        // Finds all active targets (i.e., on-screen targets).
        List<Target> activeTargets = GameManager.Instance.TargetManager.targets.FindAll(target => target.FSM.IsActive());

        foreach (Target target in activeTargets)
        {
            target.ResolveHit();
        }

        effectDuration = 0.0f;
    }

    public override void DeactivateEffect()
    {
        // throw new System.NotImplementedException();
    }
}
