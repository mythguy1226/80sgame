using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModifier : AbsModifierEffect
{
    /// <summary>
    /// Override: Activates test effect
    /// </summary>
    public override void ActivateEffect()
    {
        Debug.Log("Effect activated!");
    }
}
