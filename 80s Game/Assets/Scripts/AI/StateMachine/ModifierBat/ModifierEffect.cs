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
        RogueBat
    }

    [SerializeField]
    protected float effectDuration;
    protected float maxEffectDuration;

    [SerializeField]
    protected GameObject modifierUIPrefab;
    protected List<GameObject> modifierUIRefs;
    protected GameObject modifierUIElement;
    protected GameObject animatedModifierUIElement;

    [SerializeField]
    protected GameObject floatingTextPrefab;
    [SerializeField]
    protected string modifierName;

    protected bool bIsActive = false;
    public PlayerController activator;
    public bool bIsSelfDebuff = false;

    [SerializeField]
    protected AudioClip pickupSound;

    // Flicker fields
    [SerializeField]
    protected float lifeTime = 3.0f;
    protected float flickerTimer = 0.0f;

    // Bob lerp fields
    Vector3 bobUpPos;
    Vector3 bobDownPos;
    float bobTime = 0.0f;
    float bobDirCoef = 1.0f;

    // acquisition animation lerp field
    private float _motionValueCoefficient;

    Rigidbody2D _Rb;

    // Start is called before the first frame update
    void Awake()
    {
        InputManager.detectHitSub += ListenForShot;
        _Rb = GetComponent<Rigidbody2D>();
        modifierUIRefs = new List<GameObject>();
        maxEffectDuration = effectDuration;

        flickerTimer = lifeTime;
        bobUpPos = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        bobDownPos = transform.position;

        _motionValueCoefficient = -10.0f;
    }

    void OnDisable()
    {
        InputManager.detectHitSub -= ListenForShot;
    }

    // Called once every frame
    protected void Update()
    {
        // Manage duration timer if active
        if(bIsActive)
        {

            effectDuration -= Time.deltaTime;
            if (modifierUIRefs.Count > 0)
            {
                foreach(GameObject uiRef in modifierUIRefs)
                {
                    uiRef.transform.GetChild(0).GetComponent<Image>().fillAmount = effectDuration / maxEffectDuration;
                }
            }
            

            // Deactivate effect once timer reaches zero
            if (effectDuration <= 0.0f)
            {
                DeactivateEffect();
                CleanUp();
                
            }

            if (animatedModifierUIElement != null)
            {
                float motionValue = (_motionValueCoefficient>=0?_motionValueCoefficient:0) * Time.deltaTime * 0.15f;
                _motionValueCoefficient++;
                Debug.Log("MOD POS: " + animatedModifierUIElement.transform.position.ToString());
                Debug.Log("PLAYER POS: " + GameManager.Instance.UIManager.modifierContainers[activator.Order].transform.position.ToString());
                //animatedModifierUIElement.transform.position = Vector3.Lerp(animatedModifierUIElement.transform.position, GameManager.Instance.UIManager.modifierContainers[activator.Order].transform.position, Time.deltaTime * 4);
                animatedModifierUIElement.transform.position = Vector3.Lerp(animatedModifierUIElement.transform.position, GameManager.Instance.UIManager.modifierContainers[activator.Order].transform.position, motionValue);

                if (animatedModifierUIElement.transform.position.ToString() == GameManager.Instance.UIManager.modifierContainers[activator.Order].transform.position.ToString())
                {
                    modifierUIRefs.Remove(animatedModifierUIElement);
                    Destroy(animatedModifierUIElement);
                    AddUIRef(activator.Order);
                    _motionValueCoefficient = 0;
                }
            }
        }
        else
        {
            // Manage flicker timer for non-active mods
            flickerTimer -= Time.deltaTime;

            // Have modifier bob up and down
            transform.position = Vector3.Lerp(bobDownPos, bobUpPos, bobTime);
            bobTime += (Time.deltaTime * bobDirCoef);
            if(bobTime >= 1.0f || bobTime <= 0.0f)
                bobDirCoef *= -1.0f;

            // Destroy modifier once timer is up
            if(flickerTimer <= 0.0f)
            {
                InputManager.detectHitSub -= ListenForShot;
                Destroy(gameObject);
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

    protected void HandleModifierCountAchievement()
    {
        string key = AchievementConstants.KITTED_OUT;
        int totalModCount = AchievementManager.GetData(AchievementManager.GetAchievementByKey(key).requirementTrackingKey);
        AchievementData.TestType test = AchievementData.TestType.GreaterThanOrEqual;
        if (AchievementManager.TestUnlock(test, AchievementManager.requirements[key], totalModCount+1)) {
            AchievementManager.UnlockAchievement(AchievementConstants.KITTED_OUT);
            return;
        }
        AchievementManager.RegisterData(AchievementManager.GetAchievementByKey(key).requirementTrackingKey, totalModCount + 1);
    }

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
        if (activator == null)
        {
            Destroy(gameObject);
            return;
        }
        // Activate the effect
        bIsActive = true;
        InputManager.detectHitSub -= ListenForShot;
        ShowFloatingText();
        SoundManager.Instance.PlaySoundContinuous(pickupSound);
        GetComponent<Rigidbody2D>().gravityScale = 0.0f; // Turn off gravity here
        if (GetModType() != ModType.Confusion && GetModType() != ModType.Snail)
        {
            AnimateUIRef();
        }
        ActivateEffect();
        transform.position = new Vector3(-15.0f, 15.0f, 0.0f); // Move off-screen for duration of lifetime
        // Mods should effectively revert whatever "addShot" was made when hit
        // Don't move from here, important for avoiding race conditions
        activator.scoreController.AdjustForModShot();
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
    /// Create a UI reference of this power to be animated towards the activator
    /// </summary>
    protected void AnimateUIRef()
    {
        if (modifierUIPrefab == null)
            return;
            
        animatedModifierUIElement = Instantiate(modifierUIPrefab, this.transform.position, Quaternion.identity);
        animatedModifierUIElement.transform.position = this.transform.position;

        animatedModifierUIElement.transform.SetParent(GameManager.Instance.UIManager.pauseScreenUI.gameUIElements.transform);
        animatedModifierUIElement.transform.localScale = Vector3.one;

        modifierUIRefs.Add(animatedModifierUIElement);
    }

    /// <summary>
    /// Create a UI reference of this power for a specific player and keep track of it
    /// </summary>
    /// <param name="player">Which player needs it</param>
    protected void AddUIRef(int player)
    {
        if(modifierUIPrefab != null)
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
            //maxEffectDuration = effectDuration;
            effectDuration = maxEffectDuration;
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
            case ModType.Rewired:
                return true;
            case ModType.RogueBat:
                return true;
            case ModType.EMP:
                return true;
        }
        return false;
    }
}