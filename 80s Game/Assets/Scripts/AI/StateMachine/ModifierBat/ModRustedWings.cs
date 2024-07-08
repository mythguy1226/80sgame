public class ModRustedWings : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.RustedWings;
    }

    public override void ActivateEffect()
    {
        if (activator.HasMod(GetModType()))
        {
            activator.ExtendModDuration(GetModType(), effectDuration);
            CleanUp();
            return;
        }

        activator.SetMod(GetModType(), this);
        GameManager.Instance.isSlowed = true;
        GameManager.Instance.rustedWingsStack++;
    }

    public override void DeactivateEffect()
    {
        activator.RemoveMod(GetModType());
        GameManager.Instance.isSlowed = false;
        if(GameManager.Instance.rustedWingsStack > 0)
        {
            GameManager.Instance.rustedWingsStack--;
        }
    }
}
