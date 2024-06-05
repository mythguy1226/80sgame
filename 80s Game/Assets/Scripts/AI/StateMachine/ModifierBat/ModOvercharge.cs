

using UnityEngine;

public class ModOvercharge : AbsModifierEffect
{
    ModType thisType = ModType.Overcharge;

    /// <summary>
    /// Activate the radius expansion effect or extend a currently existing one
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
        activator.ExpandRadius();
    }


    /// <summary>
    /// Remove this modifier from the PlayerController
    /// </summary>
    public override void DeactivateEffect()
    {
        activator.RemoveMod(thisType);
        activator.ResetRadius();
    }
}