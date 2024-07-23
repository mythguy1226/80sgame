

using UnityEngine;

public class ModOvercharge : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.Overcharge;
    }
    public GameObject hitParticles;

    /// <summary>
    /// Activate the radius expansion effect or extend a currently existing one
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
        activator.ExpandRadius();
        activator.hitParticles = hitParticles;
        HandleModifierCountAchievement();
    }


    /// <summary>
    /// Remove this modifier from the PlayerController
    /// </summary>
    public override void DeactivateEffect()
    {
        activator.RemoveMod(GetModType());
        activator.ResetRadius();
        activator.hitParticles = activator.defaultHitParticles;
    }
}