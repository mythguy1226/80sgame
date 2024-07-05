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
            
            //Compose available modifiers into single list
            List<GameObject> modifierObjects = new List<GameObject>();
            foreach(GameObject buff in buffs)
            {
                modifierObjects.Add(buff);
            }
            if (!GameManager.Instance.debuffActive)
            {
                foreach(GameObject debuff in debuffs)
                {
                    modifierObjects.Add(debuff);
                }
            }
            // Alter this if you want to force a specific modifier to drop
            int randomIndex = Random.Range(0, modifierObjects.Count);
            Instantiate(modifierObjects[randomIndex], transform.position, Quaternion.identity);
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
}
