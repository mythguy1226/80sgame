using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModifier : AbsModifierEffect
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
        Debug.Log("Effect activated!");
    }

    /// <summary>
    /// Override: Deactivates test effect
    /// </summary>
    public override void DeactivateEffect()
    {
        Debug.Log("Effect deactivated!");
    }
}
