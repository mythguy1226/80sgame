using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModRustedWings : AbsModifierEffect
{
    ModType thisType = ModType.RustedWings;

    public override void ActivateEffect()
    {
        if (activator.HasMod(thisType))
        {
            activator.ExtendModDuration(thisType, effectDuration);
            CleanUp();
            return;
        }

        activator.SetMod(thisType, this);
        GameManager.Instance.isSlowed = true;
        GameManager.Instance.rustedWingsStack++;
    }

    public override void DeactivateEffect()
    {
        activator.RemoveMod(thisType);
        GameManager.Instance.isSlowed = false;
        if(GameManager.Instance.rustedWingsStack > 0)
        {
            GameManager.Instance.rustedWingsStack--;
        }
    }
}
