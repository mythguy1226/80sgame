
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

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        if (playerInput.currentControlScheme == "KnM")
        {
            sensitivity = mouseSensitivity;
        } else
        {
            sensitivity = controllerSensitivity;
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