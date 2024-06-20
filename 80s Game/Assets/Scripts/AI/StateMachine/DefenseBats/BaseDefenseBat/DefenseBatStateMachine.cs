using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenseBatStateMachine : AbsStateMachine<DefenseBatStateMachine.DefenseBatStates>
{
    // Enum used for holding all creature state keys
    public enum DefenseBatStates
    {
        Wandering,
        Pursuing,
        Attacking,
        Death
    }

    // Public fields
    public DefenseBatStates initialState = DefenseBatStates.Wandering;
    public int pointValue = 1000;
    public float deathHeight = -6.5f;
    public AudioClip hitSound;

    // Fleeing fields
    public float timeUntilPursue = 8.0f;
    public float pursueTimer = 0.0f;
    public Vector2 targetAttackLocation;

    // Get needed components for state machine
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;
    protected AnimationHandler _AnimControls;
    protected InputManager _InputManager;
    protected PlayerController stunningPlayer;
    PolygonCollider2D _Collider;


    // Public properties for bat components
    public KinematicSteer MovementControls
    {
        get { return _MovementControls; }
    }
    public SpriteRenderer SpriteRenderer
    {
        get { return _SpriteRenderer; }
    }
    public AnimationHandler AnimControls
    {
        get { return _AnimControls; }
    }
    public PolygonCollider2D Collider
    {
        get { return _Collider; }
    }

    public bool IsDefault
    {
        get { return pointValue == 1000; }
    }

    void Awake()
    {
        // Initialize all components
        Init();

        // Fill the dictionary with needed states
        states[DefenseBatStates.Wandering] = new WanderingState();
        states[DefenseBatStates.Pursuing] = new PursuingState();
        states[DefenseBatStates.Attacking] = new AttackingState();
        states[DefenseBatStates.Death] = new DefenseDeathState();

        // Default state will be wandering
        currentState = states[initialState];

        // Iterate through each state to pass game object as owner reference
        // putting this in here as well as base class to beat race condition
        foreach (KeyValuePair<DefenseBatStates, AbsBaseState<DefenseBatStates>> state in states)
        {
            state.Value.OwnerFSM = this;
        }
    }

    /// <summary>
    /// Initialize the state machine's fields and components
    /// </summary>
    void Init()
    {
        _MovementControls = GetComponent<KinematicSteer>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _AnimControls = GetComponent<AnimationHandler>();
        _Collider = GetComponent<PolygonCollider2D>();

        // Init flee timer
        pursueTimer = timeUntilPursue;
    }

    /// <summary>
    /// Sets target flee timer publicly
    /// </summary>
    public void SetPursueTimer()
    {
        pursueTimer = timeUntilPursue;
    }

    /// <summary>
    /// Resets state machine values and target values
    /// </summary>
    public virtual void Reset()
    {
        GetComponent<Target>().Reset();
        SetPursueTimer();
    }

    /// <summary>
    /// Resolves events tied to the state machine
    /// </summary>
    public override void ResolveEvent()
    {
        // Resolve hits
        ResolveHit();
    }

    /// <summary>
    /// Resolve hits
    /// </summary>
    public virtual void ResolveHit()
    {
        // Trigger stun animation
        _AnimControls.PlayStunAnimation();
        SoundManager.Instance.PlaySoundInterrupt(hitSound);
        GetComponent<Target>().bIsStunned = true;
    }

    /// <summary>
    /// Returns default state for reset purposes
    /// </summary>
    public override DefenseBatStates GetDefaultState()
    {
        return DefenseBatStates.Wandering;
    }

    /// <summary>
    /// Returns terminal/death state for reset purposes
    /// </summary>
    public override DefenseBatStates GetTerminalState()
    {
        return DefenseBatStates.Death;
    }
}
