

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputWrapper : MonoBehaviour
{
    // Class that wraps around the PlayerInput object to better handle
    // how input behaves and where it sends its messages to
    private const float sensitivityMultiplier = 1 / 150f;

    public Vector2 mouseSensitivity;
    public Vector2 controllerSensitivity;
    private Vector2 sensitivity;
    private PlayerController player;
    private PlayerInput playerInput;
    private List<Joycon> joycons;
    private float joyconSensitivyAdjust = 5.0f;
    private bool fireHeld = false;

    [SerializeField]
    private float fireDelay = 0.2f;
    private float currentDelay = 0.0f;

    public bool controllerInput;
    public bool isFlipped = false;
    public bool isSlowed = false;
    private LookingGlass lookingGlass;

    private void Start()
    {
        joycons = JoyconManager.Instance.j;
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        if (playerInput.currentControlScheme == "KnM")
        {
            controllerInput = false;
        } else
        {
            controllerInput = true;
        }

        currentDelay = fireDelay;

        SetSensitivity(controllerInput);
        lookingGlass = FindAnyObjectByType<LookingGlass>();

    }

    public void SetSensitivity(bool controllerInput)
    {
        // Set the sensitivity for this controller
        if (controllerInput)
        {
            sensitivity = controllerSensitivity;
        } 
        
        else if (joycons.Count > 0)
        {
            joycons[player.Order].SetRumble(0, 0, 0);
            sensitivity = controllerSensitivity;
        }

        else
        {
            sensitivity = mouseSensitivity;
        } 
    }


    //Handle inputs received from the Unity input system
    private void OnMove(InputValue value)
    {

        PlayerConfig config = PlayerData.activePlayers[player.Order];
        float snailModifier = 1.0f;

        // Handle slowed effect from modifiers
        if (isSlowed)
        {
            snailModifier = 0.5f;
        }
        Vector2 scalingVector = sensitivity * config.sensitivity * snailModifier * Time.deltaTime;
        Vector2 input = value.Get<Vector2>();
        if (!controllerInput)
        {
            if (input.magnitude < 1.0f)
            {
                scalingVector *= input.magnitude;
            }
        }
        
        Vector2 adjustedInput = input.normalized;
        adjustedInput = Vector2.Scale(adjustedInput, scalingVector);
        

        if (isFlipped)
        {
            adjustedInput *= -1;
        }

        // Mouse deltas are already a displacement-based value, so lag-spikes
        // will cause the cursor to jump around (No Time.deltaTime adjustments)
        player.HandleMovement(adjustedInput * sensitivityMultiplier);
    }

    private void OnNavigate(InputValue inputValue)
    {
        if (GameManager.Instance.UIManager.titleScreenUI != null)
        {
            if (GameManager.Instance.UIManager.titleScreenUI.gamemodePanel.activeInHierarchy)
            {
                GameManager.Instance.UIManager.BackgroundCycle(inputValue.Get<Vector2>());
            }
        }
        else if (GameManager.Instance.UIManager.onboardingUI != null)
        {
            if (GameManager.Instance.UIManager.onboardingUI.onboardingPanel.activeInHierarchy && PlayerData.activePlayers[player.Order].controlScheme != "KnM")
            {
                GameManager.Instance.UIManager.onboardingUI.ChangePage(inputValue.Get<Vector2>());
            }
        }
    }
        

    private void OnPrintDebug()
    {
        foreach(Target target in GameManager.Instance.TargetManager.ActiveTargets)
        {
            Debug.LogError(target.ToString());
        }
    }

    //Handle fire inputs received through the Unity input system
    private void OnFire(InputValue value)
    {


        // If the onboarding screen is on, dismiss it
        if (GameManager.Instance.UIManager.activeUI != UIManager.UIType.None)
        {

            GameManager.Instance.UIManager.GetFireInput(player.activeCrosshair.PositionToScreen());
            currentDelay = 0.0f;
        }

        fireHeld = !fireHeld;
    }


    //Received through the Unity Input system
    private void OnRecenter(InputValue value)
    {
        player.RecenterCursor();
    }

    // Override for joycons
    private void OnRecenter()
    {
        player.RecenterCursor();
    }

    // Received through the Unity input system
    private void OnPause(InputValue value)
    {
        Debug.Log("OnPause");
        player.EmitPause();
    }

    private void OnLookingGlass(InputValue value)
    {
        if (!NetworkUtility.NetworkDevEnv())
        {
            return;
        }

        // Open the debug panel if the game is paused
        if (Time.timeScale > 0f )
        {
            return;
        }

        if (!NetworkUtility.NetworkDevEnv())
        {
            return;
        }
        LookingGlassUI lookingGlassUI = FindAnyObjectByType<LookingGlassUI>();
        if (lookingGlassUI == null)
        {
            return;
        }

        lookingGlassUI.ToggleVisibility();
    }

    // Received through the Unity input system
    private void OnStartGame(InputValue value)
    {
        player.EmitPause();
    }

    // Received through the Unity input system
    private void OnCancel()
    {
        SettingsManager.Instance.CancelSettings();
        player.EmitCancel();
    }

    //Switches tabs for when settings panel is active
    private void OnPreviousTab()
    {

        SettingsManager.Instance.PreviousTab();
    }

    //Switches tabs for when settings panel is active
    private void OnNextTab()
    {
        SettingsManager.Instance.NextTab();
    }

    private void OnJoin()
    {
        if (GameManager.Instance.UIManager.onboardingUI != null && GameManager.Instance.UIManager.onboardingUI.onboardingPanel.activeInHierarchy)
        {
            GameManager.Instance.UIManager.onboardingUI.CloseOnboarding();
            return;
        }
    }

    private void OnHowToPlay()
    {
        if (GameManager.Instance.UIManager.pauseScreenUI != null && GameManager.Instance.UIManager.pauseScreenUI.isPaused)
        {
            GameManager.Instance.UIManager.pauseScreenUI.HowToPlay();
        }

        else if (GameManager.Instance.UIManager.titleScreenUI != null)
            GameManager.Instance.UIManager.pauseScreenUI.HowToPlay();
    }

    // Most of this update thing is for Joycons
    public void Update()
    {
        //Increase delay between shots
        currentDelay += Time.deltaTime;
        //If fire button is held and the delay is exceeded
        if (fireHeld && currentDelay >= fireDelay)
        {
            //Fire and reset the delay
            player.HandleFire();
            currentDelay = 0.0f;
        }
    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    public string GimmeControlScheme(int playerIndex)
    {
        return PlayerData.activePlayers[playerIndex].controlScheme;
    }

    private void OnSubmit()
    {
        Debug.Log("OnSubmit");
    }
}