using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* CLASS: BatStateMachine
 * USAGE: State Machine used for managing bat behavior and
 * storing all state keys.
 */
public class RogueBatStateMachine : AbsStateMachine<RogueBatStateMachine.RogueBatStates>
{
    // Enum used for holding all creature state keys
    public enum RogueBatStates
    {
        Observing,
        Attacking,
        Death
    }

    // Public fields
    public RogueBatStates initialState = RogueBatStates.Observing;

    // Death fields
    public float timeUntilDeath = 8.0f;
    public float deathTimer = 0.0f;
    public float deathHeight = -6.5f;

    // Get needed components for state machine
    SpriteRenderer _SpriteRenderer;
    protected AnimationHandler _AnimControls;

    // Public properties for bat components
    public SpriteRenderer SpriteRenderer
    {
        get { return _SpriteRenderer; }
    }
    public AnimationHandler AnimControls
    {
        get { return _AnimControls; }
    }

    public override bool IsDefault()
    {
        return false;
    }

    void Awake()
    {
        // Initialize all components
        Init();

        // Fill the dictionary with needed states
        states[RogueBatStates.Observing] = new ObservingState();
        states[RogueBatStates.Attacking] = new RogueAttackingState();
        states[RogueBatStates.Death] = new RogueDeathState();

        // Default state will be wandering
        currentState = states[initialState];

        // Iterate through each state to pass game object as owner reference
        // putting this in here as well as base class to beat race condition
        foreach (KeyValuePair<RogueBatStates, AbsBaseState<RogueBatStates>> state in states)
        {
            state.Value.OwnerFSM = this;
        }
    }

    /// <summary>
    /// Initialize the state machine's fields and components
    /// </summary>
    void Init()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _AnimControls = GetComponent<AnimationHandler>();

        // Init flee timer
        deathTimer = timeUntilDeath;
    }

    /// <summary>
    /// Sets target flee timer publicly
    /// </summary>
    public void SetDeathTimer()
    {
        deathTimer = timeUntilDeath;
    }

    /// <summary>
    /// Resets state machine values
    /// </summary>
    public virtual void Reset()
    {
        SetDeathTimer();
        bIsActive = false;
    }

    /// <summary>
    /// Returns default state for reset purposes
    /// </summary>
    public override RogueBatStates GetDefaultState()
    {
        return RogueBatStates.Observing;
    }

    /// <summary>
    /// Returns terminal/death state for reset purposes
    /// </summary>
    public override RogueBatStates GetTerminalState()
    {
        return RogueBatStates.Death;
    }
}