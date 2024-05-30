using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModDoublePoints : AbsModifierEffect
{
    /// <summary>
    /// Override: Activates test effect
    /// </summary>
    public override void ActivateEffect()
    {
        Debug.Log("Double Points activated!");
        activator.scoreController.pointsMod = 2;
        Debug.Log(activator.scoreController.pointsMod);
    }

    /// <summary>
    /// Override: Deactivates test effect
    /// </summary>
    public override void DeactivateEffect()
    {
        Debug.Log("Double Points deactivated!");
        activator.scoreController.pointsMod = 1;
    }
}
