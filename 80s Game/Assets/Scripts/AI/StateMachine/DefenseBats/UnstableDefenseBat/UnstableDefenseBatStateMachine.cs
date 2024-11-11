using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstableDefenseBatStateMachine : DefenseBatStateMachine
{
    // Public chain fields
    public float chainRadius = 2.0f;
    public int chainLength = 3;
    public GameObject lightning;

    /// <summary>
    /// Override: Checks if target has been stunned
    /// </summary>
    public override void ResolveHit()
    {
        // Trigger base behavior
        base.ResolveHit();
        if (!AchievementManager.isCountingUnstable())
        {
            AchievementManager.StartCountingDefenseUnstable(this);
        }
        Target myself = GetComponent<Target>();

        // Get the chain of targets
        Target[] targetChain = GetTargetChain();

        // Early return if chain is empty
        if (targetChain.Length == 0)
            return;

        // Init current target as this
        Target currentTarg = GetComponent<Target>();

        // Iterate through each chained target and play their stun animations as well
        for (int i = 0; i < targetChain.Length; i++)
        {
            if (targetChain[i] == null || targetChain[i] == myself || targetChain[i].bIsStunned)
            {
                continue;
            }

            AchievementManager.AddToUnstCount();

            // Create instance of lightning
            GameObject effect = Instantiate(lightning, currentTarg.transform.position, Quaternion.identity);
            ChainLightning chain = effect.GetComponent<ChainLightning>();

            // Play lightning effect
            chain.PlayEffect(currentTarg.transform.position, targetChain[i].transform.position);

            // Play stun anim
            try{
                DefenseBatStateMachine FSM = (DefenseBatStateMachine)targetChain[i].FSM;
                FSM.GetComponent<Target>().SetStunningPlayer(currentTarg.GetStunningPlayer());
                FSM.ResolveHit();
            }
            catch (InvalidCastException)
            {
                DebuffBatStateMachine FSM = (DebuffBatStateMachine)targetChain[i].FSM;
                FSM.GetComponent<Target>().SetStunningPlayer(currentTarg.GetStunningPlayer());
                FSM.ResolveHit();
            }
            
            //targetChain[i].GetComponent<AnimationHandler>().PlayStunAnimation();

            // Update current target
            currentTarg = targetChain[i];
        }
        AchievementManager.StopCountingDefenseUnstable(this);
    }

    /// <summary>
    /// Gets a target/bat in a radius
    /// </summary>
    /// <param name="origin">Epicenter of bat check</param>
    /// <returns>First nearby target/bat in radius</returns>
    Target GetValidTargetInRadius(Vector3 origin)
    {
        // Get all nearby targets in radius
        Collider2D[] nearbyTargets = Physics2D.OverlapCircleAll(origin, chainRadius);
        //Collider2D[] validTargets = FilterTargets(nearbyTargets);

        // If there are nearby targets then return a random one
        if (nearbyTargets.Length > 0)
        {
            // Get target and ensure target isn't a debuff bat
            Target curTarg = nearbyTargets[UnityEngine.Random.Range(0, nearbyTargets.Length - 1)].gameObject.GetComponent<Target>();
            if(curTarg != null)
                return curTarg;
        }

        return null;
    }

    /// <summary>
    /// Gets a target chain
    /// </summary>
    /// <returns>Chain of targets/bats</returns>
    Target[] GetTargetChain()
    {
        // Initialize the target chain
        Target[] targetChain = new Target[chainLength];

        // Get the current target
        Target currentTarget = GetValidTargetInRadius(transform.position);

        // Iterate by number of chains and add to array
        for (int i = 0; i < chainLength; i++)
        {
            // Set index to current target
            targetChain[i] = currentTarget;

            // Get the next target
            if(targetChain[i] != null)
                currentTarget = GetValidTargetInRadius(targetChain[i].transform.position);
        }

        // Return full chain
        return targetChain;
    }
}
