

using UnityEngine;

public class ModOvercharge : AbsModifierEffect
{
    ModType thisType = ModType.Overcharge;
    public GameObject hitParticles;

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
        activator.hitParticles = hitParticles;
    }


    /// <summary>
    /// Remove this modifier from the PlayerController
    /// </summary>
    public override void DeactivateEffect()
    {
        activator.RemoveMod(thisType);
        activator.ResetRadius();
        activator.hitParticles = activator.defaultHitParticles;
    }
}