using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierDefenseBatStateMachine : DefenseBatStateMachine
{
    // Public modifier fields
    List<GameObject> buffs;
    List<GameObject> debuffs;
    bool modifierDropped = false;

    /// <summary>
    /// Override: Start method
    /// </summary>
    void Start()
    {
        // Get buffs and debuffs from game manager
        buffs = GameManager.Instance.buffs;
        debuffs = GameManager.Instance.debuffs;
    }

    /// <summary>
    /// Override: Checks if target has been stunned
    /// </summary>
    public override void ResolveHit()
    {
        // Trigger base behavior
        base.ResolveHit();

        // Instantiate the modifier object
        if (!modifierDropped)
        {
            List<GameObject> buffs = GameManager.Instance.GetBuffs();
            List<GameObject> debuffs = GameManager.Instance.GetDebuffs();

            // Compose available modifiers into single list and make list of weights from config
            List<GameObject> modifierObjects = new List<GameObject>();
            List<float> weights = new List<float>();

            for(int i = 0; i < buffs.Count; i++)
            {
                // Add to list of buffs and weighted list if unlocked
                if(GameManager.Instance.ActiveGameMode.isModifierAllowed(buffs[i].GetComponent<AbsModifierEffect>().GetModType()))
                {
                    modifierObjects.Add(buffs[i]);
                    weights.Add(GameManager.Instance.weightConfig.weights[i].chance);
                }
            }
            if (!GameManager.Instance.debuffActive || GameManager.Instance.ActiveGameMode.isInDebugMode())
            {
                foreach(GameObject debuff in debuffs)
                {
                    modifierObjects.Add(debuff);
                }
            }

            // Guard Clause
            if (modifierObjects.Count == 0)
            {
                return;
            }

            // Calculate weighted index
            int weightedIndex = GetRandomWeightedIndex(weights);
            if(weightedIndex >= modifierObjects.Count || weightedIndex < 0)
                weightedIndex = 0;

            Instantiate(modifierObjects[weightedIndex], transform.position, Quaternion.identity);
            modifierDropped = true;
        }
    }

    /// <summary>
    /// Override: Resets target
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        // Reset drop flag
        modifierDropped = false;
    }

    /// <summary>
    /// Credits: Algorithm written by Andy Gainey, and edited to work
    /// with our solution for weighted modifiers.
    /// Link here: https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/ 
    ///
    /// Method returns a random weighted index which takes in the list
    /// of weights for the calculation
    /// </summary>
    public int GetRandomWeightedIndex(List<float> weights)
    {
        // Get the total sum of all the weights.
        float weightSum = 0.0f;
        for (int i = 0; i < weights.Count; ++i)
        {
            weightSum += weights[i];
        }
     
        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Count - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }
     
            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }
     
        // No other item was selected, so return very last index.
        return index;
    }
}
