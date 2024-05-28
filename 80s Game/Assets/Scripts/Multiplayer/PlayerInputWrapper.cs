
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputWrapper : MonoBehaviour
{
    public Vector2 mouseSensitivity;
    public Vector2 controllerSensitivity;
    private Vector2 sensitivity;
    private PlayerController player;
    private PlayerInput playerInput;
    private List<Joycon> joycons;
    private float joyconSensitivyAdjust = 5.0f;

    public bool controllerInput;
    
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

        SetSensitivity(controllerInput);
    }

    public void SetSensitivity(bool controllerInput)
    {
        if (controllerInput)
        {
            sensitivity = controllerSensitivity;
        } 
        
        else if (joycons.Count > 0)
        {
            joycons[player.Order-1].SetRumble(0, 0, 0);
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
        PlayerConfig config = PlayerData.activePlayers[player.Order - 1];
        Vector2 adjustedInput = Vector2.Scale(value.Get<Vector2>(), sensitivity * config.sensitivity);
        player.HandleMovement(adjustedInput);

    }

    private void OnMove(Vector2 value)
    {
        Vector2 adjustedInput = Vector2.Scale(value, sensitivity * joyconSensitivyAdjust);
        player.HandleMovement(adjustedInput);
    }

    private void OnFire(InputValue value)
    {
        player.HandleFire();
    }

    private void OnFire()
    {
        player.HandleFire();
    }

    private void OnRecenter(InputValue value)
    {
        player.RecenterCursor();
    }

    private void OnRecenter()
    {
        player.RecenterCursor();
    }

    private void OnPause(InputValue value)
    {
        player.EmitPause();
    }

    private void OnPause()
    {

    }

    private void OnCancel()
    {
        SettingsManager.Instance.CancelSettings();
    }

    public void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[player.Order-1];

            // Gyro values: x, y, z axis values (in radians per second)
            Vector3 gyro = j.GetGyro();

            // Update cursor position based on gyroscope values
            OnMove(new Vector2(gyro.z, gyro.y));

            // Get right trigger input
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                OnFire();
            }

            //shoulderPressed = true;
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                OnRecenter();
            }

            if ((j.GetButtonDown(Joycon.Button.PLUS) || j.GetButtonDown(Joycon.Button.MINUS)) && Time.timeScale > 0)
            {
                OnPause();
            }
        }

        if (playerInput.currentControlScheme == "KnM")
        {
            controllerInput = false;
            SetSensitivity(controllerInput);
        } else
        {
            controllerInput = true;
            SetSensitivity(controllerInput);
        }
    }
}