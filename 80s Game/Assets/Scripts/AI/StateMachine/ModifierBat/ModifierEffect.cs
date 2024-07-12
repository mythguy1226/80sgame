using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public abstract class AbsModifierEffect : MonoBehaviour
{
    public enum ModType
    {
        DoublePoints = 0,
        Overcharge,
        Snail,
        Confusion,
        RustedWings,
        EMP,
        Rewired,
    }

    [SerializeField]
    protected float effectDuration;
    protected float maxEffectDuration;

    [SerializeField]
    protected GameObject modifierUIPrefab;
    protected List<GameObject> modifierUIRefs;
    protected GameObject modifierUIElement;

    [SerializeField]
    protected GameObject floatingTextPrefab;
    [SerializeField]
    protected string modifierName;

    protected bool bIsActive = false;
    protected PlayerController activator;

    Rigidbody2D _Rb;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.detectHitSub += ListenForShot;
        _Rb = GetComponent<Rigidbody2D>();
        modifierUIRefs = new List<GameObject>();
        maxEffectDuration = effectDuration;
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

            effectDuration -= Time.deltaTime;
            if (modifierUIElement)
            {
                modifierUIElement.transform.GetChild(0).GetComponent<Image>().fillAmount = effectDuration / maxEffectDuration;
            }
            

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
        ShowFloatingText();
        _Rb.gravityScale = 0.0f; // Turn off gravity here
        AddUIRef(activator.Order);
        ActivateEffect();
        transform.position = new Vector3(-15.0f, 15.0f, 0.0f); // Move off-screen for duration of lifetime
    }

    /// <summary>
    /// Clean this object up
    /// </summary>
    protected void CleanUp()
    {
        bIsActive = false;
        Destroy(gameObject);
        foreach(GameObject uiRef in modifierUIRefs)
        {
            Destroy(uiRef);
        }
    }

    /// <summary>
    /// Create a UI reference of this power for a specific player and keep track of it
    /// </summary>
    /// <param name="player">Which player needs it</param>
    protected void AddUIRef(int player)
    {
        if(modifierUIElement != null)
        {
            modifierUIElement = GameManager.Instance.UIManager.CreateModifierUI(modifierUIPrefab, player);
            modifierUIRefs.Add(modifierUIElement);
        }
    }

    /// <summary>
    /// Add to the duration of this effect
    /// </summary>
    /// <param name="value">How much time to add</param>
    public void AddDuration(float value)
    {
        effectDuration += value;

        if (effectDuration > maxEffectDuration)
        {
            maxEffectDuration = effectDuration;
        }
    }

    /// <summary>
    /// Display floating text at modifier location
    /// </summary>
    private void ShowFloatingText()
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        text.GetComponent<TextMeshPro>().text = $"{modifierName}";
    }

    /// <summary>
    /// Gets the mod type for a concrete class
    /// </summary>
    /// <returns>The mod type</returns>
    public abstract ModType GetModType();


    /// <summary>
    /// Static function to test whether a modtype is a buff or not
    /// </summary>
    /// <param name="type">The type to test</param>
    /// <returns>Whether or not this type is a buff</returns>
    public static bool ModTypeIsBuff(ModType type)
    {
        switch (type)
        {
            case ModType.DoublePoints:
                return true;
            case ModType.Overcharge:
                return true;
            case ModType.Snail:
                return false;
            case ModType.Confusion:
                return false;
            case ModType.RustedWings:
                return true;
        }
        return false;
    }
}