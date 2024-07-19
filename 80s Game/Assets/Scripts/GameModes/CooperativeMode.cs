using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CooperativeMode : AbsGameMode
{
    float modifierChance = 0.3f;

    public CooperativeMode() : base()
    {
        ModeType = EGameMode.Competitive;

        // Initial round parameters
        NumRounds = 99;
        maxTargetsOnScreen = 15;
        currentRoundTargetCount = 8;
        allowedBats = new Dictionary<TargetManager.TargetType, bool>();
        SetupAllowedData();
        debugMode = false;

        // Get number of players to pass into scaling method
        int playerCount = GameManager.Instance.GetPlayerCount();
        ScaleGameValues(playerCount);
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
        allowedBuffs[AbsModifierEffect.ModType.Overcharge] = true;
        allowedBuffs[AbsModifierEffect.ModType.RustedWings] = true;
        allowedDebuffs[AbsModifierEffect.ModType.Confusion] = true;
        allowedDebuffs[AbsModifierEffect.ModType.Snail] = true;

        allowedBats[TargetManager.TargetType.Regular] = true;
        allowedBats[TargetManager.TargetType.Modifier] = true;
        allowedBats[TargetManager.TargetType.Unstable] = true;
        allowedBats[TargetManager.TargetType.DiveBomb] = true;

        numBatsMap = new Dictionary<TargetManager.TargetType, int>();

        // Init map types
        numBatsMap.Add(TargetManager.TargetType.Regular, 0);
        numBatsMap.Add(TargetManager.TargetType.Modifier, 0);
        numBatsMap.Add(TargetManager.TargetType.DiveBomb, 0);
        numBatsMap.Add(TargetManager.TargetType.Unstable, 0);
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
                targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<DefenseBatStateMachine>());
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
        currentRoundTargetCount += 4;
        maxTargetsOnScreen += 1;

        GameManager.Instance.UIManager.scoreBehavior.ShowNewRoundText();
        
        // Keep max targets on screen to at most two fewer than object pool
        if (maxTargetsOnScreen >= targetManager.targets.Count)
        {
            maxTargetsOnScreen = targetManager.targets.Count - 2;
        }

        targetManager.numStuns = 0;
        targetManager.UpdateTargetParams();
        if (GameManager.Instance.roundEndTheme != null)
            SoundManager.Instance.PlayNonloopMusic(GameManager.Instance.roundEndTheme);
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
        if (targetManager.numStuns >= currentRoundTargetCount && allowedBats[TargetManager.TargetType.Modifier])
        {
            StartNextRound();

            // Spawn a modifier bat and increment target count
            targetManager.SpawnTarget(targetManager.GetNextAvailableTargetOfType<ModifierDefenseBatStateMachine>());
            currentRoundTargetCount++;

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
}
