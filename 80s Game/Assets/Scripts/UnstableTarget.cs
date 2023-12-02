using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstableTarget : Target
{
    // Public chain fields
    public float chainRadius = 2.0f;
    public int chainLength = 3;
    public GameObject lightning;

    // Method for checking if target has been stunned
    protected override void DetectStun()
    {
        // Check time scale so bats cant be harmed while game is paused
        if (inputManager.MouseLeftDownThisFrame && Time.timeScale > 0)
        {
            // Check for player input coords hitting target
            Vector3 shotPos = inputManager.MouseWorldPosition;
            RaycastHit2D hit = Physics2D.Raycast(shotPos, Vector2.zero);

            // Check if something was hit
            if (!hit) 
            { 
                //SoundManager.Instance.PlaySound(missSound); 
                return; 
            }

            // Check that hit has detected this particular object
            if (hit.collider.gameObject == gameObject)
            {
                animControls.PlayStunAnimation();
                SoundManager.Instance.PlaySoundInterrupt(hitSound);

                // Get the chain of targets
                Target[] targetChain = GetTargetChain();

                // Early return if chain is empty
                if (targetChain.Length == 0)
                    return;

                // Init current target as this
                Target currentTarg = this;

                // Iterate through each chained target and play their stun animations as well
                for (int i = 0; i < targetChain.Length; i++)
                {
                    // Create instance of lightning
                    GameObject effect = Instantiate(lightning, currentTarg.transform.position, Quaternion.identity);
                    ChainLightning chain = effect.GetComponent<ChainLightning>();
                    
                    // Play lightning effect
                    chain.PlayEffect(currentTarg.transform.position, targetChain[i].transform.position);

                    // Play stun anim
                    targetChain[i].GetComponent<AnimationHandler>().PlayStunAnimation();

                    // Update current target
                    currentTarg = targetChain[i];
                }
            }
        }
    }

    // Method for getting a target in a radius
    Target GetTargetInRadius(Vector3 origin)
    {
        // Get all nearby targets in radius
        Collider2D[] nearbyTargets = Physics2D.OverlapCircleAll(origin, chainRadius);

        // If there are nearby targets then return a random one
        if (nearbyTargets.Length > 0)
            return nearbyTargets[Random.Range(0, nearbyTargets.Length - 1)].gameObject.GetComponent<Target>();

        return null;
    }

    // Method for getting a target chain
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
            currentTarget = GetTargetInRadius(targetChain[i].transform.position);
        }

        // Return full chain
        return targetChain;
    }
}
