using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsModifierEffect : MonoBehaviour
{
    public enum ModType
    {
        DoublePoints,
        Overcharge,
        Snail,
        Confusion
    }

    [SerializeField]
    protected float effectDuration;
    
    [SerializeField]
    protected GameObject modifierUIPrefab;
    private GameObject modifierUIRef;

    protected bool bIsActive = false;
    protected PlayerController activator;

    Rigidbody2D _Rb;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.detectHitSub += ListenForShot;
        _Rb = GetComponent<Rigidbody2D>();
    }

    void OnDisable()
    {
        InputManager.detectHitSub -= ListenForShot;
    }

    // Called once every frame
    void Update()
    {
        // Manage duration timer if active
        if(bIsActive)
        {
            Debug.Log(effectDuration);
            effectDuration -= Time.deltaTime;

            // Deactivate effect once timer reaches zero
            if (effectDuration <= 0.0f)
            {
                DeactivateEffect();
                CleanUp();
                
            }
        }

        // Detect when modifier has fallen out of world
        if(transform.position.y <= -10.0f && !bIsActive)
        {
            InputManager.detectHitSub -= ListenForShot;
            Destroy(gameObject);
        }    
    }

    /// <summary>
    /// Abstract method each modifier class will implement
    /// to implement modifier effects
    /// </summary>
    public abstract void ActivateEffect();

    /// <summary>
    /// Abstract method each modifier class will implement
    /// to implement deactivation of effects
    /// </summary>
    public abstract void DeactivateEffect();

    /// <summary>
    /// Checks if modifier has been shot
    /// </summary>
    public bool DetectHit(Vector3 pos)
    {
        bool isGameGoing = Time.timeScale > 0;
        if (!isGameGoing)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        // Null check
        if (!hit)
            return false;

        // Check that hit has detected this particular object
        if (hit.collider.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Listener event for detecting hits from input manager
    /// </summary>
    /// <param name="position">Position where player last fired</param>
    public void ListenForShot(ShotInformation s)
    {
        // Detect hit at location
        if(DetectHit(s.location))
        {
            // Ensure activator is set
            if (activator == null)
                activator = s.player;

            ResolveShot();
        }
    }

    /// <summary>
    /// Handler for shot resolution
    /// </summary>
    public void ResolveShot()
    {
        // Activate the effect
        bIsActive = true;
        InputManager.detectHitSub -= ListenForShot;
        transform.position = new Vector3(-15.0f, 15.0f, 0.0f); // Move off-screen for duration of lifetime
        _Rb.gravityScale = 0.0f; // Turn off gravity here
        modifierUIRef = GameManager.Instance.UIManager.CreateModifierUI(modifierUIPrefab, activator.Order);
        ActivateEffect();
    }

    protected void CleanUp()
    {
        bIsActive = false;
        Destroy(gameObject);
        Destroy(modifierUIRef);
    }

    /// <summary>
    /// Add to the duration of this effect
    /// </summary>
    /// <param name="value">How much time to add</param>
    public void AddDuration(float value)
    {
        effectDuration += value;
    }
}