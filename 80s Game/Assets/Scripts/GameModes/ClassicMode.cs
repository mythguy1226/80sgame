using System;
using System.Collections.Generic;
using System.Linq;



public class ClassicMode : AbsGameMode
{

    public ClassicMode() : base()
    {
        ModeType = EGameMode.Classic;

        // Initial round parameters
        NumRounds = 10;
        maxTargetsOnScreen = 8;
        currentRoundTargetCount = 5;

        SetupAllowedData();
        debugMode = false;
    }

    protected override void SetupAllowedData()
    {
        // Initialize data stores
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();
        allowedBuffs = new Dictionary<AbsModifierEffect.ModType, bool>();
        allowedDebuffs = new Dictionary<AbsModifierEffect.ModType, bool>();

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
            } else
            {
                allowedDebuffs.Add(mod, false);
            }
        }

        // Enable the right ones for this game mode
        allowedBuffs[AbsModifierEffect.ModType.DoublePoints] = true;
        allowedBuffs[AbsModifierEffect.ModType.Overcharge] = true;

        allowedBats[TargetManager.TargetType.Regular] = true;
        allowedBats[TargetManager.TargetType.HighBonus] = false;
        allowedBats[TargetManager.TargetType.LowBonus] = false;
        allowedBats[TargetManager.TargetType.Modifier] = false;
        allowedBats[TargetManager.TargetType.Unstable] = false;

        numBatsMap = new Dictionary<TargetManager.TargetType, int>();

        // Init map types
        numBatsMap.Add(TargetManager.TargetType.Regular, 0);
        numBatsMap.Add(TargetManager.TargetType.Modifier, 0);
        numBatsMap.Add(TargetManager.TargetType.LowBonus, 0);
        numBatsMap.Add(TargetManager.TargetType.HighBonus, 0);
        numBatsMap.Add(TargetManager.TargetType.Unstable, 0);
        numBatsMap.Add(TargetManager.TargetType.Debuff, 0);
    }

    protected override void StartNextRound(bool isFirstRound = false)
    {
        if (!isFirstRound)
            
            UpdateRoundParams();

        // Iterate through and spawn the next set of targets
        for(int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();
            
            if (targetIndex == -1 && allowedBats[TargetManager.TargetType.Regular])
            {
                targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                if(targetIndex != -1)
                    targetManager.SpawnTarget(targetIndex);
                continue;
            }

            targetManager.SpawnTarget(targetIndex);

            // Check if screen is now full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }
    }

    protected override void UpdateRoundParams()
    {
        CurrentRound++;
        currentRoundTargetCount += 2;
        maxTargetsOnScreen += 1;
        
        // Keep max targets on screen to at most two fewer than object pool
        if(maxTargetsOnScreen >= targetManager.targets.Count)
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
            if (SkipBat(bat))
            {
                continue;
            }

            // Check for special bat types
            foreach(SpawnRate rate in GameManager.Instance.spawnConfig.rates)
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

        if(targetManager.numStuns >= currentRoundTargetCount)
        {

            GameManager.EmitRoundOverEvent();

            // If last round completed
            if(CurrentRound == NumRounds)
            {
                GameOver = true;
                EndGame();
            }
            // Otherwise start next round
            else
            {
                // Play round-end jingle and call method for delayed round start
                if (GameManager.Instance.roundEndTheme != null)
                    SoundManager.Instance.PlaySoundContinuous(GameManager.Instance.roundEndTheme.Clip);
                GameManager.Instance.UIManager.scoreBehavior.ShowNewRoundText();
                GameManager.Instance.StartRoundDelay();
            }
                

            return;
        }

        // If not the end of a round, check if more targets can be spawned
        SpawnMoreTargets();
    }

    protected override void EndGame()
    {
        int score = GameManager.Instance.PointsManager.maxScore;
        AchievementManager.TestEndGameAchievements(ModeType, CurrentRound, score);
        AchievementManager.BullseyeTest(GameManager.Instance.GetPlayer(0));
        GameManager.Instance.HandleGameOver();
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
            {
                targetIndex = targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.Regular);
                if(targetIndex != -1)
                    targetManager.SpawnTarget(targetIndex);
                else
                    targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<BatStateMachine>());
            }
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
            targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierBatStateMachine>());
            currentRoundTargetCount++;
        }

        // Spawn some bonus bats and increment target count
        for(int i = 0; i < 2; i++)
        {
            if (allowedBats[TargetManager.TargetType.LowBonus])
            {
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.LowBonus));
                currentRoundTargetCount++;
            }
            if (allowedBats[TargetManager.TargetType.HighBonus])
            {
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfEnumType(TargetManager.TargetType.HighBonus));
                currentRoundTargetCount++;
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
            case 2:
                allowedBats[TargetManager.TargetType.Unstable] = true;
                break;
            case 4:
                allowedBats[TargetManager.TargetType.LowBonus] = true;
                allowedBats[TargetManager.TargetType.HighBonus] = true;
                break;
            case 5:
                allowedBats[TargetManager.TargetType.Modifier] = true;
                break;
            case 6:
                allowedBats[TargetManager.TargetType.Debuff] = true;
                break;
        }
    }
}
