using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CompetitiveMode : AbsGameMode
{
    float modifierChance = 0.3f;

    public CompetitiveMode() : base()
    {
        ModeType = EGameMode.Competitive;

        // Initial round parameters
        NumRounds = 12;
        maxTargetsOnScreen = 15;
        currentRoundTargetCount = 8;
        SetupAllowedData();
        debugMode = false;
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
        allowedBuffs[AbsModifierEffect.ModType.DoublePoints] = true;
        allowedBuffs[AbsModifierEffect.ModType.RustedWings] = true;
        if(AchievementManager.GetAchievementByKey("mod-1").isUnlocked())
            allowedBuffs[AbsModifierEffect.ModType.Overcharge] = true;

        allowedBats[TargetManager.TargetType.Regular] = true;
        allowedBats[TargetManager.TargetType.LowBonus] = true;
        allowedBats[TargetManager.TargetType.HighBonus] = true;
        allowedBats[TargetManager.TargetType.Modifier] = true;
        allowedBats[TargetManager.TargetType.Unstable] = true;
        allowedBats[TargetManager.TargetType.Debuff] = true;

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
        for (int i = 0; i < currentRoundTargetCount; i++)
        {
            int targetIndex = GetNextAvailableBat();

            if (targetIndex == -1 && allowedBats[TargetManager.TargetType.Regular])
            {
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<BatStateMachine>());
                continue;
            }

            targetManager.SpawnTarget(targetIndex);

            // Check if screen is now full
            if (targetManager.ActiveTargets.Count == maxTargetsOnScreen)
                return;
        }

        // Spawn a modifier bat and increment target count
        if (allowedBats[TargetManager.TargetType.Modifier])
        {
            targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierBatStateMachine>());
        }
        currentRoundTargetCount++;
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

        // Chance to spawn a modifier bat every 5 stuns
        if(targetManager.totalStuns % 5 == 0)
        {
            if(UnityEngine.Random.Range(0.0f, 1.0f) < modifierChance && allowedBats[TargetManager.TargetType.Modifier])
            {
                // Spawn a modifier bat and increment target count
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierBatStateMachine>());
                currentRoundTargetCount++;
            }
        }

        if (targetManager.numStuns >= currentRoundTargetCount)
        {

            // If last round completed
            if (CurrentRound == NumRounds)
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
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<BatStateMachine>());
        }
    }

    protected override void CallNextRound()
    {
        // Begin the next round
        StartNextRound();
    }
}
