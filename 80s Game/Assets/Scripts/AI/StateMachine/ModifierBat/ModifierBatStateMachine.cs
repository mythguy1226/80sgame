using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class ModifierBatStateMachine : BatStateMachine
{
    // Public modifier fields
    public List<GameObject> modifierObjects;
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
            // Alter this if you want to force a specific modifier to drop
            // i.e. if you want the Overcharged to always to drop, set the array access in line 27 to 0
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
