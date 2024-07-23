using System;
using System.Collections;
using System.Collections.Generic;


public class ModDoublePoints : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.DoublePoints;
    }

    /// <summary>
    /// Override: Activates test effect
    /// </summary>
    public override void ActivateEffect()
    {
        if (activator.HasMod(GetModType()))
        {
            activator.ExtendModDuration(GetModType(), effectDuration);
            CleanUp();
            return;
        }

        activator.SetMod(GetModType(), this);
        activator.scoreController.pointsMod = 2;
        HandleModifierCountAchievement();
    }

    /// <summary>
    /// Override: Deactivates test effect
    /// </summary>
    public override void DeactivateEffect()
    {
        activator.scoreController.pointsMod = 1;
        activator.RemoveMod(GetModType());
    }
}
