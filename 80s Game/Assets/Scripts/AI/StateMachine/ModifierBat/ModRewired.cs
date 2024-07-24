using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModRewired : AbsModifierEffect
{
    public float effectRadius = 5.0f;
    public GameObject particles;

    public List<GameObject> particlesList;

    public override ModType GetModType()
    {
        return ModType.Rewired;
    }

    public override void ActivateEffect()
    {
        List<Target> targetsInRadius = GameManager.Instance.TargetManager.ActiveTargets.FindAll(target => Vector2.Distance(transform.position, target.transform.position) <= effectRadius);

        foreach (Target target in targetsInRadius)
        {
            // Create and then destroy particles after time
            DefenseBatStateMachine dFSM = (target.FSM as DefenseBatStateMachine);

            // Protect against dead but still technically "active" bats
            if (dFSM.CurrentState.StateKey != DefenseBatStateMachine.DefenseBatStates.Death)
            {
                GameObject particle = Instantiate(particles, target.transform);
                particlesList.Add(particle);
                target.rewiredParticles = particle;
                dFSM.TransitionToState(DefenseBatStateMachine.DefenseBatStates.Wandering);
                dFSM.AnimControls.ResetAnimation();
                dFSM.pursueTimer = effectDuration;
            }
        }
        HandleModifierCountAchievement();
    }

    public override void DeactivateEffect()
    {
        foreach(GameObject particle in particlesList)
        {
            if(particle != null)
            {
                Destroy(particle);
            }
        }
    }
}
