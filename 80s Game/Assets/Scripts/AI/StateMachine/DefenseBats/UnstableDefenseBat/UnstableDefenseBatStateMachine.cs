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
            if (targetChain[i] == null)
            {
                continue;
            }

            // Create instance of lightning
            GameObject effect = Instantiate(lightning, currentTarg.transform.position, Quaternion.identity);
            ChainLightning chain = effect.GetComponent<ChainLightning>();

            // Play lightning effect
            chain.PlayEffect(currentTarg.transform.position, targetChain[i].transform.position);

            // Play stun anim
            targetChain[i].GetComponent<Target>().SetStunningPlayer(stunningPlayer);
            targetChain[i].GetComponent<AnimationHandler>().PlayStunAnimation();

            // Update current target
            currentTarg = targetChain[i];
        }
    }

    /// <summary>
    /// Gets a target/bat in a radius
    /// </summary>
    /// <param name="origin">Epicenter of bat check</param>
    /// <returns>First nearby target/bat in radius</returns>
    Target GetTargetInRadius(Vector3 origin)
    {
        // Get all nearby targets in radius
        Collider2D[] nearbyTargets = Physics2D.OverlapCircleAll(origin, chainRadius);

        // If there are nearby targets then return a random one
        if (nearbyTargets.Length > 0)
            return nearbyTargets[Random.Range(0, nearbyTargets.Length - 1)].gameObject.GetComponent<Target>();

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
        Target currentTarget = GetTargetInRadius(transform.position);

        // Iterate by number of chains and add to array
        for (int i = 0; i < chainLength; i++)
        {
            // Set index to current target
            targetChain[i] = currentTarget;

            // Get the next target
            if(targetChain[i] != null)
                currentTarget = GetTargetInRadius(targetChain[i].transform.position);
        }

        // Return full chain
        return targetChain;
    }
}
