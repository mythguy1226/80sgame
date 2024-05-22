
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

    bool controllerInput;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        if (playerInput.currentControlScheme == "PS4")
        {
            controllerInput = true;
        } else
        {
            controllerInput = false;
        }

        controllerSensitivity = new Vector2(PlayerPrefs.GetFloat("GamepadXSensitivity"),PlayerPrefs.GetFloat("GamepadYSensitivity"));
        mouseSensitivity = new Vector2(PlayerPrefs.GetFloat("MouseXSensitivity"),PlayerPrefs.GetFloat("MouseYSensitivity"));

        SetSensitivity(controllerInput);
    }

    public void SetSensitivity(bool controllerInput)
    {
        if (controllerInput)
        {
            sensitivity = controllerSensitivity;
        } else
        {
            sensitivity = mouseSensitivity;
        }
    }

    //Handle inputs received from the Unity input system
    private void OnMove(InputValue value)
    {
        Vector2 adjustedInput = Vector2.Scale(value.Get<Vector2>(), sensitivity);
        player.HandleMovement(adjustedInput);
    }

    private void OnFire(InputValue value)
    {
        player.HandleFire();
    }

    private void OnRecenter(InputValue value)
    {

    }

    private void OnPause(InputValue value)
    {

    }
}