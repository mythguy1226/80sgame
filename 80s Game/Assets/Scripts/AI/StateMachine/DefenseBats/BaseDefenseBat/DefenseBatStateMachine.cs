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

    /// <summary>
    /// Returns the closest available latch position for the bat to target
    /// </summary>
    public LatchPoint GetClosestLatchPoint()
    {
        // Get reference to all defendables
        List<Defendable> defendables = new List<Defendable>(GameObject.FindObjectsOfType<Defendable>());

        // Init closest defendable
        Defendable closestDefendable = defendables[0];

        // Iterate through all defendables to get the closest one
        foreach(Defendable curDefend in defendables)
        {
            // Only include active defendables
            if(!curDefend.bCanBeTargeted)
                continue;

            // Only target core if its the only defendable left to attack
            List<Defendable> activeDefend = GetActiveDefendables();
            if(curDefend.bIsCore && activeDefend.Count > 1)
                continue;

            // Distance check and availability check
            if(Vector3.Distance(transform.position, curDefend.transform.position) < Vector3.Distance(transform.position, closestDefendable.transform.position))
            {
                closestDefendable = curDefend;
            }
        }

        // Get the next available latch on the closest defendable
        LatchPoint nextLatch = null;
        if(closestDefendable.bCanBeTargeted)
            nextLatch = GetAvailableLatch(closestDefendable);

        // Return the closest latch
        return nextLatch;
    }

    /// <summary>
    /// Returns the next available latch on a given defendable
    /// </summary>
    public LatchPoint GetAvailableLatch(Defendable target)
    {
        // Iterate through all latches in defendable
        foreach(LatchPoint curLatch in target.latchPoints)
        {
            // Return the next available latch
            if(curLatch.isAvailable)
                return curLatch;
        }

        return null;
    }

    public List<Defendable> GetActiveDefendables()
    {
        // Init list
        List<Defendable> activeList = new List<Defendable>();

        // Iterate through all defendables and only add active ones
        foreach(Defendable curDefend in GameObject.FindObjectsOfType<Defendable>())
        {
            // Check activity
            if(curDefend.bCanBeTargeted)
                activeList.Add(curDefend);
        }

        // Return list
        return activeList;
    }

    /// <summary>
    /// Overridable method used for executing bat attack logic
    /// </summary>
    public virtual void Attack()
    {
        // Get the defendable from the target latch
        Defendable latchedDefendable = targetLatch.transform.parent.GetComponent<Defendable>();
        AnimControls.PlayAttackAnimation();

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
