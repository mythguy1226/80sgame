using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Pursue fields
    public float timeUntilPursue = 8.0f;
    public float pursueTimer = 0.0f;
    public LatchPoint targetLatch;
    public float pursueSpeedScale = 1.0f;
    public bool bCanPursue = false;

    // Attack fields
    public float attackCooldown = 0.5f;
    float attackTimer;
    bool bCanAttack = true;
    public int attackDamage = 1;

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

    public override bool IsDefault()
    {
        return pointValue == 1000;
    }

    public float AttackTimer
    {
        get { return attackTimer; }
        set { attackTimer = value; }
    }

    public bool CanAttack
    {
        get { return bCanAttack; }
        set { bCanAttack = value; }
    }

    void Awake()
    {
        // Initialize all components
        Init();

        // Fill the dictionary with needed states
        states[DefenseBatStates.Wandering] = new WanderingState();
        states[DefenseBatStates.Pursuing] = new PursuingState();
        states[DefenseBatStates.Attacking] = new AttackingState(_AnimControls);
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

        // Reset any latch points the bat is attached to
        if(targetLatch != null)
        {
            targetLatch.Unlatch();
            targetLatch = null;
        }

        bCanPursue = false;
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
        GetComponent<Target>().bIsStunned = true;
        _AnimControls.PlayStunAnimation();
        SoundManager.Instance.PlaySoundInterrupt(hitSound, 0.9f, 1.1f);
        
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

    /// <summary>
    /// Returns the closest available latch position for the bat to target
    /// </summary>
    public LatchPoint GetClosestLatchPoint()
    {
        List<Defendable> activeDefendables = GetActiveDefendables()
            // Order by distance to target
            .OrderBy(def => Vector3.Distance(transform.position, def.transform.position))
            .ToList<Defendable>();

        if (activeDefendables.Count <= 0)
            return null;

        Defendable closestDef = null;
        if(activeDefendables.Count > 1)
            closestDef = activeDefendables.Find(def => !def.bIsCore);
        else
            closestDef = activeDefendables[0];

        return GetAvailableLatch(closestDef);
    }

    /// <summary>
    /// Returns the next available latch on a given defendable
    /// </summary>
    public LatchPoint GetAvailableLatch(Defendable target)
    {
        return target.latchPoints.Find(latch => latch.isAvailable);
    }

    public List<Defendable> GetActiveDefendables()
    {
        return GameObject.FindObjectsOfType<Defendable>()
            .ToList<Defendable>()
            .FindAll(defendable => defendable.bCanBeTargeted);
    }

    /// <summary>
    /// Overridable method used for executing bat attack logic
    /// </summary>
    public virtual void Attack()
    {
        Target target = GetComponent<Target>();
        if (target.bIsStunned)
        {
            target.ResolveHit();
        }

        // Test for bats having been unlatched
        if (targetLatch == null)
        {
            return;
        }

        // Get the defendable from the target latch
        Defendable latchedDefendable = targetLatch.transform.parent.GetComponent<Defendable>();
        // AnimControls.PlayAttackAnimation(); Moved to AttackingState

        // Deal damage
        latchedDefendable.TakeDamage(attackDamage);
    }

    /// <summary>
    /// Overridable method called when transitioning to pursue state
    /// </summary>
    public virtual void BeginPursue()
    {
        bCanPursue = true;
    }
}
