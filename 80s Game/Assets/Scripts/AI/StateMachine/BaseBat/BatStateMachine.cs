using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* CLASS: BatStateMachine
 * USAGE: State Machine used for managing bat behavior and
 * storing all state keys.
 */
public class BatStateMachine : AbsStateMachine<BatStateMachine.BatStates>
{
    // Enum used for holding all creature state keys
    public enum BatStates
    {
        Moving,
        Fleeing,
        Death
    }

    // Public fields
    public BatStates initialState = BatStates.Moving;
    public int pointValue = 1000;
    public float deathHeight = -6.5f;
    public AudioClip hitSound;

    // Fleeing fields
    public float timeUntilFlee = 8.0f;
    public float fleeTimer = 0.0f;
    public Vector2 fleeLocation;

    // Get needed components for state machine
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;
    protected AnimationHandler _AnimControls;
    protected InputManager _InputManager;
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

    public override bool IsDefault()
    {
        return pointValue == 1000;
    }

    void Awake()
    {
        // Initialize all components
        Init();

        // Fill the dictionary with needed states
        states[BatStates.Moving] = new MovingState();
        states[BatStates.Fleeing] = new FleeingState();
        states[BatStates.Death] = new DeathState();

        // Default state will be wandering
        currentState = states[initialState];

        // Iterate through each state to pass game object as owner reference
        // putting this in here as well as base class to beat race condition
        foreach (KeyValuePair<BatStates, AbsBaseState<BatStates>> state in states)
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
        fleeTimer = timeUntilFlee;
    }

    /// <summary>
    /// Sets target flee timer publicly
    /// </summary>
    public void SetFleeTimer()
    {
        fleeTimer = timeUntilFlee;
    }

    /// <summary>
    /// Resets state machine values and target values
    /// </summary>
    public virtual void Reset()
    {
        GetComponent<Target>().Reset();
        SetFleeTimer();
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
    public override BatStates GetDefaultState()
    {
        return BatStates.Moving;
    }

    /// <summary>
    /// Returns terminal/death state for reset purposes
    /// </summary>
    public override BatStates GetTerminalState()
    {
        return BatStates.Death;
    }
}
