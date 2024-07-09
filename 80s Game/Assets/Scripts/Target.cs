using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public TargetManager.TargetType type;

    // Get state machine
    public StateMachineWrapper FSM;

    // Get needed components for state machine
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;
    protected AnimationHandler _AnimControls;
    protected InputManager _InputManager;
    protected PlayerController stunningPlayer;
    PolygonCollider2D _Collider;

    // Default fields used for resets
    Vector3 spawnPoint;
    public bool bIsStunned = false;

    // Public fields
    public GameObject floatingTextPrefab;
    public int pointValue = 1000;
    public float deathHeight = -6.5f;
    public AudioClip hitSound;

    // Public fields
    public bool IsActive 
    {
        get { return FSM.IsActive(); }
    }

    public float MovementSpeed
    {
        get { return _MovementControls.maxSpeed; }
    }

    // Call once upon start of game
    void Awake()
    {
        // Init component references
        Init();
    }

    void OnDisable()
    {
        InputManager.detectHitSub -= ListenForShot;
    }

    /// <summary>
    /// Initialize the target's fields and components
    /// </summary>
    void Init()
    {
        FSM = GetComponent<StateMachineWrapper>();
        _MovementControls = GetComponent<KinematicSteer>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _AnimControls = GetComponent<AnimationHandler>();
        _Collider = GetComponent<PolygonCollider2D>();

        spawnPoint = transform.position;
    }

    /// <summary>
    /// Updates movement control speed
    /// </summary>
    /// <param name="newSpeed">New speed</param>
    public void UpdateSpeed(float newSpeed)
    {
        _MovementControls.maxSpeed = newSpeed;
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
        FSM.TransitionToTerminal();
        _AnimControls.PlayDropAnimation();
        _Collider.isTrigger = true;
        GetComponent<CircleCollider2D>().isTrigger = true;
        
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

    /// <summary>
    /// Handles event of bat being stunned
    /// </summary>
    public void ResolveHit()
    {
        FSM.ResolveEvent();
    }

    public void Spawn()
    {
        FSM.SetActive(true);
        InputManager.detectHitSub += ListenForShot;
        FSM.TransitionToDefault();
    }

    /// <summary>
    /// Resets the bat
    /// </summary>
    public void Reset()
    {
        // The particle systems drag across the screen when repositioning bats
        // Stopping it when resetting prevents that
        if (GetComponentInChildren<ParticleSystem>())
        {
            GetComponentInChildren<ParticleSystem>().Stop();
        }

        // Set bat to its default values
        FSM.SetActive(false);
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
        if (!FSM.InUnscorableState())
        {
            // Add a successful hit
            if(stunningPlayer != null)
            {
                gameManager.TargetManager.AddToCount(GetComponent<Target>().type, gameManager.TargetManager.killCount);
                stunningPlayer.scoreController.AddHit();
                gameManager.PointsManager.AddRoundPoints(stunningPlayer.Order, pointValue * stunningPlayer.scoreController.pointsMod);
            }
        }

        // Update target manager with current state
        gameManager.TargetManager.OnTargetReset();
        InputManager.detectHitSub -= ListenForShot;
        stunningPlayer = null;
    }

    public void SetStunningPlayer(PlayerController controller)
    {
        stunningPlayer = controller;
    }

    public PlayerController GetStunningPlayer()
    {
        return stunningPlayer;
    }

    /// <summary>
    /// Display floating text at bat location
    /// </summary>
    private void ShowFloatingText()
    {
        //Don't Show Floating Text in Defense Mode
        if (GameModeData.activeGameMode == EGameMode.Defense)
        {
            return;
        }

        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);

        if (stunningPlayer != null)
        {
            if (stunningPlayer.HasMod(AbsModifierEffect.ModType.DoublePoints))
            {
                text.GetComponent<TextMeshPro>().text = $"{pointValue * 2}";
            }

            else
            {
                text.GetComponent<TextMeshPro>().text = $"{pointValue}";
            }

            if (GameModeData.activeGameMode == EGameMode.Competitive)
            {
                text.GetComponent<TextMeshPro>().color = stunningPlayer.activeCrosshair.gameObject.GetComponent<SpriteRenderer>().color;
            }
        }
    }
}
