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
    public GameObject floatingTextPrefab;
    public int pointValue = 1000;
    public float deathHeight = -6.5f;
    public AudioClip hitSound;
    //public AudioClip missSound;

    // Fleeing fields
    public float timeUntilFlee = 8.0f;
    public float fleeTimer = 0.0f;
    public Vector2 fleeLocation;

    // Default fields used for resets
    Vector3 spawnPoint;
    bool bIsStunned = false;

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
        stunningPlayer = null;

        // Iterate through each state to pass game object as owner reference
        // putting this in here as well as base class to beat race condition
        foreach (KeyValuePair<DefenseBatStates, AbsBaseState<DefenseBatStates>> state in states)
        {
            state.Value.OwnerFSM = this;
        }
    }

    void Start()
    {
        
    }

    void OnDisable()
    {
        InputManager.detectHitSub -= ListenForShot;
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
        spawnPoint = transform.position;

        // Init flee timer
        fleeTimer = timeUntilFlee;
    }

    /// <summary>
    /// Updates movement control speed
    /// </summary>
    /// <param name="newSpeed">New speed</param>
    public void UpdateSpeed(float newSpeed)
    {
        _MovementControls.maxSpeed = newSpeed;
    }

    /// <summary>
    /// Sets target flee timer publicly
    /// </summary>
    public void SetFleeTimer()
    {
        fleeTimer = timeUntilFlee;
    }


    private bool DetectRadius(Vector3 pos, float radius)
    {
        // Check if this target is within shot radius
        float distance = Vector3.Distance(pos, transform.position);
        if (distance > radius)
            return false;

        // Check that hit has detected this particular object
        if (distance <= radius && !bIsStunned)
            return true;

        return false;
    }
    private bool DetectCollision(Vector3 pos)
    {
        // Check if something was hit
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (!hit)
            return false;

        // Check that hit has detected this particular object
        if (hit.collider.gameObject == gameObject && !bIsStunned)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if bat has been stunned
    /// </summary>
    public bool DetectStun(Vector3 pos, bool radCheck, float radius)
    {

        // Check that game isnt paused
        bool isGameGoing = Time.timeScale > 0;
        if (!isGameGoing)
        {
            return false;
        }

        if (radCheck)
        {
            return DetectRadius(pos, radius);
        }

        return DetectCollision(pos);
        
    }

    /// <summary>
    /// Drops the bat
    /// </summary>
    public void DropBat()
    {
        if (floatingTextPrefab != null)
        {
            ShowFloatingText();
        }

        // Bring bat to death state to start falling
        TransitionToState(DefenseBatStates.Death);
        _AnimControls.PlayDropAnimation();
        _Collider.isTrigger = true;
        GetComponent<CircleCollider2D>().isTrigger = true;
        
    }

    /// <summary>
    /// Display floating text at bat location
    /// </summary>
    private void ShowFloatingText()
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        text.GetComponent<TextMeshPro>().text = $"{pointValue}";
    }

    /// <summary>
    /// Resets the bat
    /// </summary>
    public virtual void Reset()
    {
        // The particle systems drag across the screen when repositioning bats
        // Stopping it when resetting prevents that
        if (GetComponentInChildren<ParticleSystem>())
        {
            GetComponentInChildren<ParticleSystem>().Stop();
        }

        // Set bat to its default values
        bIsActive = false;
        transform.position = spawnPoint;
        _MovementControls.canMove = false;
        _AnimControls.ResetAnimation();
        _Collider.isTrigger = false;
        GetComponent<CircleCollider2D>().isTrigger = false;
        bIsStunned = false;


        // Choose new wander position to be used on respawn
        _MovementControls.SetWanderPosition();

        // Get reference to game manager instance
        GameManager gameManager = GameManager.Instance;

        // Add points if target didn't flee
        if (currentState != states[DefenseBatStates.Pursuing])
        {
            // Add a successful hit
            gameManager.TargetManager.AddToCount(GetComponent<Target>().type, gameManager.TargetManager.killCount);
            stunningPlayer.scoreController.AddHit();
            gameManager.PointsManager.AddRoundPoints(stunningPlayer.Order, pointValue * stunningPlayer.scoreController.pointsMod);
        }

        // Update target manager with current state
        gameManager.TargetManager.OnTargetReset();
        InputManager.detectHitSub -= ListenForShot;
        stunningPlayer = null;
    }

    /// <summary>
    /// Stun event listener attached to input manager
    /// </summary>
    /// <param name="s">Struct containing information about shot</param>
    public void ListenForShot(ShotInformation s)
    {
        // Check for stun detection
        if(DetectStun(s.location, s.isRadiusCheck, s.player.GetShotRadius()))
        {
            // Allow this to only be set once to prevent players from snipe stealing points from each other
            if (stunningPlayer == null)
            {
                stunningPlayer = s.player;
            }
            // Call method for stun resolution
            ResolveHit();
        }
    }

    public void Spawn()
    {
        bIsActive = true;
        InputManager.detectHitSub += ListenForShot;
        TransitionToState(DefenseBatStates.Wandering);
        SetFleeTimer();
    }

    /// <summary>
    /// Overridable: Handles event of bat being stunned
    /// </summary>
    public virtual void ResolveHit()
    {
        // Trigger stun animation
        _AnimControls.PlayStunAnimation();
        SoundManager.Instance.PlaySoundInterrupt(hitSound);
        bIsStunned = true;
    }

    public void SetStunningPlayer(PlayerController controller)
    {
        stunningPlayer = controller;
    }
}
