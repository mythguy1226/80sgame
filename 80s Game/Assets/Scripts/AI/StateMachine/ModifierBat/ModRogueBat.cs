using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModRogueBat : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.RogueBat;
    }

    public GameObject rogueBat;

    public override void ActivateEffect()
    {
        // Spawn a new rogue bat
        Instantiate(rogueBat, transform.position, Quaternion.identity);
    }

    public override void DeactivateEffect()
    {
        // Nothing really needed here
        // Rogue Bat AI state machine handles clean up
    }
}
