using System.Collections.Generic;
using System;

[Serializable]
public class BatData
{
    // Data class that keeps track of Bat information.
    // Ideally there would be a way of updating this automatically rather
    // than having to manually maintain this as we update bat types

    public int normal;
    public int unstable;
    public int lowbonus;
    public int highbonus;
    public int modified;

    public BatData(Dictionary<TargetManager.TargetType, int> data)
    {
        normal = 0;
        unstable = 0;
        lowbonus = 0;
        highbonus = 0;
        modified = 0;
        LoadData(data);
    }

    /// <summary>
    /// Loads data directly from a dictionary counter kept by some other class
    /// </summary>
    /// <param name="data">The counter object kept by another class</param>
    private void LoadData(Dictionary<TargetManager.TargetType, int> data)
    {
        // All of these methods test for keys because they are lazily added by the counters
        // No key existence is guaranteed
        if (data.ContainsKey(TargetManager.TargetType.Regular))
        {
            normal = data[TargetManager.TargetType.Regular];
        }

        if (data.ContainsKey(TargetManager.TargetType.LowBonus))
        {
            lowbonus = data[TargetManager.TargetType.LowBonus];
        }

        if (data.ContainsKey(TargetManager.TargetType.HighBonus))
        {
            highbonus = data[TargetManager.TargetType.HighBonus];
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
    // Data class that keeps track of Modifier information.
    // Ideally there would be a way of updating this automatically rather
    // than having to manually maintain this as we update modifier types

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

    /// <summary>
    /// Loads data directly from a dictionary counter kept by some other class
    /// </summary>
    /// <param name="data">The counter object kept by another class</param>
    private void LoadData(Dictionary<AbsModifierEffect.ModType, int> data)
    {
        // All of these methods test for keys because they are lazily added by the counters
        // No key existence is guaranteed
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