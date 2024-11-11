using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameMode
{
    Classic,
    Competitive,
    Defense,
    Menu
}

public abstract class AbsGameMode
{
    public EGameMode ModeType {  get; protected set; }
    public bool GameOver { get; protected set; }
    public int CurrentRound {  get; protected set; }

    // Should be initialized in sub-class
    public int NumRounds { get; protected set; }

    protected TargetManager targetManager;
    protected int maxTargetsOnScreen;
    protected int currentRoundTargetCount;
    protected int numBonusBats = 0;
    protected Dictionary<TargetManager.TargetType, bool> allowedBats;
    protected Dictionary<AbsModifierEffect.ModType, bool> allowedBuffs;
    protected Dictionary<AbsModifierEffect.ModType, bool> allowedDebuffs;
    protected bool debugMode;

    protected Dictionary<TargetManager.TargetType, int> numBatsMap;

    public AbsGameMode()
    {
        targetManager = GameManager.Instance.TargetManager;
        GameOver = false;
        CurrentRound = 1;
    }

    public void StartGame()
    {
        StartNextRound(true);
    }

    protected abstract void EndGame();

    /// <summary>
    /// Validation function to determine if this bat should be skipped for spawning
    /// </summary>
    /// <param name="target">Target to evaluate</param>
    /// <returns>Whether or not to skip this target for spawning</returns>
    protected bool SkipBat(Target target)
    {
        return !allowedBats[target.type] || target.FSM.IsActive() || target.type == TargetManager.TargetType.Modifier;
    }

    /// <summary>
    /// Validation function to determine if this bat should be skipped for spawning in defense mode
    /// </summary>
    /// <param name="target">Target to evaluate</param>
    /// <returns>Whether or not to skip this target for spawning</returns>
    protected bool SkipDefenseBat(Target target)
    {
        return !allowedBats[target.type] || target.FSM.IsActive() || target.type == TargetManager.TargetType.Modifier;
    }

    // Needs to be public for targetManager to call
    public abstract void OnTargetReset();

    protected abstract int GetNextAvailableBat();

    protected abstract void StartNextRound(bool isFirstRound = false);

    protected abstract void CallNextRound();

    protected abstract void UpdateRoundParams();

    protected abstract void SetupAllowedData();

    /// <summary>
    /// Returns if a bat type can spawn in a concrete game mode
    /// </summary>
    /// <param name="type">Type of bat to check</param>
    /// <returns>Whether or not it can spawn</returns>
    public bool CanSpawnType(TargetManager.TargetType type)
    {
        return allowedBats[type];
    }

    /// <summary>
    /// Toggles whether a bat is allowed to spawn or not. Useful for debugging
    /// </summary>
    /// <param name="type">The type to toggle</param>
    public void ToggleAllowedBatType(TargetManager.TargetType type)
    {
        allowedBats[type] = !allowedBats[type];
        debugMode = true;
    }

    public void ToggleAllowedModType(AbsModifierEffect.ModType type)
    {
        debugMode = true;
        if (AbsModifierEffect.ModTypeIsBuff(type))
        {
            allowedBuffs[type] = !allowedBuffs[type];
        } else
        {
            allowedDebuffs[type] = !allowedDebuffs[type];
        }
        GameManager.Instance.UpdateModifiers();
    }

    public bool isModifierAllowed(AbsModifierEffect.ModType type)
    {
        if (AbsModifierEffect.ModTypeIsBuff(type))
        {
            return allowedBuffs[type];
        }
        return allowedDebuffs[type];
    }

    public bool isInDebugMode()
    {
        return debugMode;
    }

    /// <summary>
    /// Coroutine used for delaying beginning of next round
    /// </summary>
    public IEnumerator DelayNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        CallNextRound();
    }
}
