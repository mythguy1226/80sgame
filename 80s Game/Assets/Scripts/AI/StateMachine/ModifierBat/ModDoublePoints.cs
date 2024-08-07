using System;
using System.Collections;
using System.Collections.Generic;


public class ModDoublePoints : AbsModifierEffect
{
    ModType thisType = ModType.DoublePoints;

    /// <summary>
    /// Override: Activates test effect
    /// </summary>
    public override void ActivateEffect()
    {
        if (activator.HasMod(thisType))
        {
            activator.ExtendModDuration(thisType, effectDuration);
            CleanUp();
            return;
        }

        activator.SetMod(thisType, this);
        activator.scoreController.pointsMod = 2;
    }

    /// <summary>
    /// Override: Deactivates test effect
    /// </summary>
    public override void DeactivateEffect()
    {
        activator.scoreController.pointsMod = 1;
        activator.RemoveMod(thisType);
    }
}
