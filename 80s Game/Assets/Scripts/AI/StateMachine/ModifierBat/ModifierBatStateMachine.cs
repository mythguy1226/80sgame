using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBatStateMachine : BatStateMachine
{
    // Public modifier fields
    public GameObject modifierObject;
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
            Instantiate(modifierObject, transform.position, Quaternion.identity);
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
