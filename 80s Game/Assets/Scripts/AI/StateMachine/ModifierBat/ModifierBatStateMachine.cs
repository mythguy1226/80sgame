using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModifierBatStateMachine : BatStateMachine
{
    // Public modifier fields
    public List<GameObject> buffs;
    public List<GameObject> debuffs;
    bool modifierDropped = false;

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
