using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebuffBatStateMachine : BatStateMachine
{
    /// <summary>
    /// Override: Start method
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Override: Checks if target has been stunned
    /// </summary>
    public override void ResolveHit()
    {
        // Trigger base behavior
        base.ResolveHit();

        List<GameObject> debuffs = GameManager.Instance.selfDebuffs;

        //Compose available modifiers into single list
        List<GameObject> modifierObjects = new List<GameObject>();
        foreach(GameObject debuff in debuffs)
        {
            modifierObjects.Add(debuff);
        }

        // Calculate random index
        int randomIndex = Random.Range(0, modifierObjects.Count); 
        
        // Spawn modifier object and its effect
        GameObject modObj = Instantiate(modifierObjects[randomIndex], transform.position, Quaternion.identity);
        AbsModifierEffect effect = modObj.GetComponent<AbsModifierEffect>();

        // Set the activator for the effect to be stunning player
        effect.activator = GetComponent<Target>().stunningPlayer;
        effect.bIsSelfDebuff = true;
    }

    /// <summary>
    /// Override: Resets target
    /// </summary>
    public override void Reset()
    {
        base.Reset();
    }
}
