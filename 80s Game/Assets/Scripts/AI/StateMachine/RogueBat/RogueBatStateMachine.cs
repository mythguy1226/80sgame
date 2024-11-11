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

    // Combat fields
    public Target currentTarget;
    public GameObject projectileClass;
    public Transform shootLocation;
    public Transform leftShotLoc;
    public Transform rightShotLoc;
    public float shotCooldown = 1.0f;
    float shotTimer;
    bool canShoot = true;

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

    public float ShotTimer
    {
        get { return shotTimer; }
        set { shotTimer = value; }
    }

    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }

    public override bool IsDefault()
    {
        return false;
    }

    public override string GetCurrentState()
    {
        return currentState.ToString();
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

        // Init shot location to be on the left
        shootLocation = leftShotLoc;
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
        Destroy(gameObject);
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

    /// <summary>
    /// Get the target that is currently closest in distance to this bat
    /// </summary>
    /// <returns>Closest target to the bat</returns>
    public Target GetClosestTarget()
    {
        // Get all active targets
        List<Target> targets = GameManager.Instance.TargetManager.ActiveTargets;

        // Only proceed if list is not empty
        if(targets.Count > 0)
        {
            // Find the closest target to the bat
            Target tempClosest = targets[0];
            foreach(Target curTarg in targets)
            {
                // Distance check and choose the closest one
                if(Vector3.Distance(transform.position, tempClosest.transform.position) > Vector3.Distance(transform.position, curTarg.transform.position))
                {
                    tempClosest = curTarg;
                }
            }
            return tempClosest;
        }

        return null;
    }

    /// <summary>
    /// Launches a stun projectile to hit nearby bats
    /// </summary>
    public void LaunchProjectile()
    {
        // Instantiate the projectile and set its velocity
        GameObject projectile = Instantiate(projectileClass, shootLocation.position, transform.rotation);

        projectile.GetComponent<Rigidbody2D>().velocity = (currentTarget.transform.position - transform.position) * projectile.GetComponent<Projectile>().projectileSpeed;
    }
}
