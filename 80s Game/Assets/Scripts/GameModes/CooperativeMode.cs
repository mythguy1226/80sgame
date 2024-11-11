using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SteamIntegration;

public class CooperativeMode : AbsGameMode
{
    //float modifierChance = 0.3f;
    Defendable coreObject;
    public CooperativeMode() : base()
    {
        ModeType = EGameMode.Defense;

        // Initial round parameters
        NumRounds = 99;
        maxTargetsOnScreen = 15;
        currentRoundTargetCount = 8;
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();
        SetupAllowedData();
        debugMode = false;

        // Find the core
        List<Defendable> defendables = new List<Defendable>(GameObject.FindObjectsOfType<Defendable>());
        foreach(Defendable defendable in defendables)
        {
            if (defendable.bIsCore)
            {
                coreObject = defendable;
                break;
            }
        }

        // Get number of players to pass into scaling method
        int playerCount = GameManager.Instance.GetPlayerCount();
        ScaleGameValues(playerCount);
        GameManager.Instance.SteamInterface.InitData();
    }

    protected override void SetupAllowedData()
    {
        allowedBuffs = new Dictionary<AbsModifierEffect.ModType, bool>();
        allowedDebuffs = new Dictionary<AbsModifierEffect.ModType, bool>();
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();

        // Set everything to false by default
        foreach (TargetManager.TargetType type in Enum.GetValues(typeof(TargetManager.TargetType)).Cast<TargetManager.TargetType>())
        {
            allowedBats.Add(type, false);
        }
        foreach (AbsModifierEffect.ModType mod in Enum.GetValues(typeof(AbsModifierEffect.ModType)).Cast<AbsModifierEffect.ModType>())
        {
            if (AbsModifierEffect.ModTypeIsBuff(mod))
            {
                allowedBuffs.Add(mod, false);
            }
            else
            {
                allowedDebuffs.Add(mod, false);
            }
        }

        // Enable the right ones for this game mode
        allowedBuffs[AbsModifierEffect.ModType.Rewired] = true;
        allowedBuffs[AbsModifierEffect.ModType.RustedWings] = true;
        if(AchievementManager.GetAchievementByKey("mod-1").isUnlocked())
            allowedBuffs[AbsModifierEffect.ModType.Overcharge] = true;
        if(AchievementManager.GetAchievementByKey("mod-2").isUnlocked())
            allowedBuffs[AbsModifierEffect.ModType.RogueBat] = true;
        if(AchievementManager.GetAchievementByKey("mod-3").isUnlocked())
            allowedBuffs[AbsModifierEffect.ModType.EMP] = true;

        allowedBats[TargetManager.TargetType.Regular] = true;
        allowedBats[TargetManager.TargetType.Modifier] = false;
        allowedBats[TargetManager.TargetType.Unstable] = false;
        allowedBats[TargetManager.TargetType.DiveBomb] = false;
        allowedBats[TargetManager.TargetType.Debuff] = false;

        numBatsMap = new Dictionary<TargetManager.TargetType, int>();

        // Init map types
        numBatsMap.Add(TargetManager.TargetType.Regular, 0);
        numBatsMap.Add(TargetManager.TargetType.Modifier, 0);
        numBatsMap.Add(TargetManager.TargetType.DiveBomb, 0);
        numBatsMap.Add(TargetManager.TargetType.Unstable, 0);
        numBatsMap.Add(TargetManager.TargetType.Debuff, 0);
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
            UpdateRoundParams();

        // Iterate through and spawn the next set of targets
        for (int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();

            if (targetIndex == -1 && allowedBats[TargetManager.TargetType.Regular])
            {
                if(CurrentRound <= 5)
                {
                    targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                    if(targetIndex != -1)
                        targetManager.SpawnTarget(targetIndex);
                }
                else
                {
                    targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<DefenseBatStateMachine>());
                }
                continue;
            }

            targetManager.SpawnTarget(targetIndex);

            // Check if screen is now full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }

        string key = AchievementConstants.CAREFUL_FRAGILE;
        AchievementData.TestType testType = AchievementData.TestType.LessThanOrEqual;
        float testValue = (float) coreObject._currentHitpoints / (float) coreObject._maxHitpoints;
        if (AchievementManager.TestUnlock(testType, AchievementManager.requirements[key], testValue*100))
        {
            AchievementManager.UnlockAchievement(key);
        }
    }

    protected override void UpdateRoundParams()
    {
        CurrentRound++;
        currentRoundTargetCount += 4;
        maxTargetsOnScreen += 1;
        
        // Keep max targets on screen to at most two fewer than object pool
        if (maxTargetsOnScreen >= targetManager.targets.Count)
        {
            maxTargetsOnScreen = targetManager.targets.Count - 2;
        }

        targetManager.numStuns = 0;
        targetManager.UpdateTargetParams();
    }

    protected override int GetNextAvailableBat()
    {
        List<Target> bats = targetManager.targets;

        // Iterate through the targets until you
        // find one that isn't already on screen
        for (int i = 0; i < bats.Count; i++)
        {
            Target bat = bats[i];
            // Debug Override
            if (debugMode && allowedBats[bat.type] && !bat.FSM.IsActive())
            {
                return i;
            }
            if (SkipDefenseBat(bat))
            {
                continue;
            }

            // Check for special bat types
            foreach (SpawnRate rate in GameManager.Instance.spawnConfig.rates)
            {
                // Check that type isnt regular
                if(rate.targetType != TargetManager.TargetType.Regular)
                {
                    // Check that there are special types available
                    if(numBatsMap[rate.targetType] > 0)
                    {
                        // Check for proper type and continue if not
                        if(bat.FSM.IsDefault() || bat.type != rate.targetType)
                            goto EndLoop;

                        numBatsMap[rate.targetType]--;
                        return i;
                    }
                }
            }

            // If default bat return index, as there were no available
            // special bats to spawn previously
            if (bat.FSM.IsDefault())
            {
                return i;
            }

        EndLoop:
            continue;
        }

        return -1;
    }

    public override void OnTargetReset()
    {
        // Check rates map to see if any special bats should
        // be added to it
        foreach(SpawnRate rate in GameManager.Instance.spawnConfig.rates)
        {
            if(rate.targetType != TargetManager.TargetType.Regular)
            {
                if(targetManager.totalStuns % rate.spawnRate == 0)
                    numBatsMap[rate.targetType]++;
            }
        }

        // Start the next round if desired number of stuns is met
        if (targetManager.numStuns >= currentRoundTargetCount)
        {
            // Play round-end jingle and call method for delayed round start
            if (GameManager.Instance.roundEndTheme != null)
                SoundManager.Instance.PlaySoundContinuous(GameManager.Instance.roundEndTheme.Clip);
            GameManager.Instance.UIManager.scoreBehavior.ShowNewRoundText();
            GameManager.Instance.StartRoundDelay();
            return;
        }

        // If not the end of a round, check if more targets can be spawned
        SpawnMoreTargets();
    }

    private void SpawnMoreTargets()
    {
        // Check if the player still needs stuns for the round
        int neededStuns = currentRoundTargetCount - targetManager.numStuns;
        if (neededStuns <= 0)
            return;

        // If maximum number of targets isn't on screen
        if (
            targetManager.ActiveTargets.Count < maxTargetsOnScreen
            && targetManager.ActiveTargets.Count < neededStuns
        )
        {
            int targetIndex = GetNextAvailableBat();

            if (targetIndex >= 0)
                targetManager.SpawnTarget(targetIndex);
            else if (!allowedBats[TargetManager.TargetType.Regular])
                return;
            else
                targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                if(targetIndex != -1)
                    targetManager.SpawnTarget(targetIndex);
                else
                    targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<DefenseBatStateMachine>());
        }
    }

    /// <summary>
    /// Method calls end game logic and is public so it may be callable
    /// from defendable class when it detects core is destroyed
    /// </summary>
    public void EndCoopGame()
    {
        GameOver = true;
        EndGame();
    }

    protected override void EndGame()
    {
        int score = GameManager.Instance.PointsManager.maxScore;
        GameManager.Instance.SteamInterface.SetSteamData(SteamConstants.DEFENSE_ROUNDS, CurrentRound, true);
        GameManager.Instance.SteamInterface.UpdateSteamServer();
        AchievementManager.TestEndGameAchievements(ModeType, CurrentRound, score);
        GameManager.Instance.HandleGameOver();
    }

    /// <summary>
    /// Method takes in number of players and updates initial
    /// values to scale. The more players the more difficulty
    /// the game is balanced to.
    /// </summary>
    public void ScaleGameValues(int playerCount)
    {
        // Subtract 1 from count value for scale
        int valueScale = playerCount - 1;

        // Increase target counts by value scale
        maxTargetsOnScreen += (2 * valueScale);
        currentRoundTargetCount += (2 * valueScale);

        // Modify target values based on scale
        List<Target> bats = targetManager.targets;
        for (int i = 0; i < bats.Count; i++)
        {
            // Affect only defense bats
            DefenseBatStateMachine dFSM = bats[i].GetComponent<DefenseBatStateMachine>();
            if(dFSM == null)
                continue;

            // Decrease timers/cooldowns and increase speed
            dFSM.timeUntilPursue -= (0.5f * valueScale);
            dFSM.attackCooldown -= (0.5f * valueScale);
            dFSM.pursueSpeedScale += (0.1f * valueScale);
        }

    }

    protected override void CallNextRound()
    {
        // Incorporate new bats based on round
        UpdateAllowedBats();

        // Begin the next round
        StartNextRound();

        // Spawn a modifier bat and increment target count
        if (allowedBats[TargetManager.TargetType.Modifier])
        {
            targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierDefenseBatStateMachine>());
            currentRoundTargetCount++;
        }

        // Spawn more modifier bats during rounds 3 and 4 to introduce mods as a mechanic
        if(CurrentRound == 3 || CurrentRound == 4)
        {
            // Spawn some debuff bats and increment target count
            for(int i = 0; i < 2; i++)
            {
                if (allowedBats[TargetManager.TargetType.Modifier])
                {
                    targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierDefenseBatStateMachine>());
                    currentRoundTargetCount++;
                }
            }
        }

        // Spawn some debuff bats and increment target count
        for(int i = 0; i < 2; i++)
        {
            if (allowedBats[TargetManager.TargetType.Debuff])
            {
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<DebuffBatStateMachine>());
                currentRoundTargetCount++;
            }
        }
    }

    public void UpdateAllowedBats()
    {
        // Check round numbers and unlock specific bat types at specific rounds
        switch(CurrentRound)
        {
            case 1:
                allowedBats[TargetManager.TargetType.Unstable] = true;
                allowedBats[TargetManager.TargetType.DiveBomb] = true;
                break;
            case 2:
                allowedBats[TargetManager.TargetType.Unstable] = false;
                allowedBats[TargetManager.TargetType.DiveBomb] = false;
                allowedBats[TargetManager.TargetType.Modifier] = true;
                break;
            case 4:
                allowedBats[TargetManager.TargetType.Debuff] = true;
                break;
            case 5:
                allowedBats[TargetManager.TargetType.DiveBomb] = true;
                allowedBats[TargetManager.TargetType.Unstable] = true;
                break;
        }
    }
}
