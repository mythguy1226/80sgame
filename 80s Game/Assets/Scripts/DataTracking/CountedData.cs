using System.Collections.Generic;
using System;

[Serializable]
public class BatData
{
    public int normal;
    public int unstable;
    public int bonus;
    public int modified;

    public BatData()
    {
        normal = 0;
        unstable = 0;
        bonus = 0;
        modified = 0;
    }

    public BatData(Dictionary<TargetManager.TargetType, int> data)
    {
        normal = 0;
        unstable = 0;
        bonus = 0;
        modified = 0;
        LoadData(data);
    }

    private void LoadData(Dictionary<TargetManager.TargetType, int> data)
    {
        if (data.ContainsKey(TargetManager.TargetType.Regular))
        {
            normal = data[TargetManager.TargetType.Regular];
        }

        if (data.ContainsKey(TargetManager.TargetType.Bonus))
        {
            bonus = data[TargetManager.TargetType.Bonus];
        }

        if (data.ContainsKey(TargetManager.TargetType.Unstable))
        {
            unstable = data[TargetManager.TargetType.Unstable];
        }

        if (data.ContainsKey(TargetManager.TargetType.Modifier))
        {
            modified = data[TargetManager.TargetType.Modifier];
        }

    }
}

[Serializable]
public class ModifierData
{
    public int snail;
    public int overcharged;
    public int doublePoints;
    public int confusion;

    public ModifierData(Dictionary<AbsModifierEffect.ModType, int> count)
    {
        snail = 0;
        overcharged = 0;
        doublePoints = 0;
        confusion = 0;

        LoadData(count);
    }

    private void LoadData(Dictionary<AbsModifierEffect.ModType, int> data)
    {
        if (data.ContainsKey(AbsModifierEffect.ModType.Confusion))
        {
            confusion = data[AbsModifierEffect.ModType.Confusion];
        }

        if (data.ContainsKey(AbsModifierEffect.ModType.Snail))
        {
            snail = data[AbsModifierEffect.ModType.Snail];
        }

        if (data.ContainsKey(AbsModifierEffect.ModType.DoublePoints))
        {
            doublePoints = data[AbsModifierEffect.ModType.DoublePoints];
        }

        if (data.ContainsKey(AbsModifierEffect.ModType.Overcharge))
        {
            overcharged = data[AbsModifierEffect.ModType.Overcharge];
        }

    }
}