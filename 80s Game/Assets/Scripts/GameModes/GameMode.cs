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

    public AbsGameMode()
    {
        targetManager = GameManager.Instance.TargetManager;
        GameOver = false;
        CurrentRound = 1;
    }

    // Needs to be public for targetManager to call
    public abstract void OnTargetReset();

    protected abstract int GetNextAvailableBat();

    protected abstract void StartNextRound(bool isFirstRound = false);

    protected abstract void UpdateRoundParams();
}
