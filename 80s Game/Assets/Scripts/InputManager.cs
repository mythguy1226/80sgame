using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Properties
    public bool MouseLeftDown
    {
        get;
        private set;
    }

    public bool MouseLeftDownThisFrame
    {
        get;
        private set;
    }

    // Update is called once per frame
    public void Update()
    {
        // The "new" Unity Input System's mouse state getter
        Mouse mouse = Mouse.current;

        MouseLeftDown = mouse.leftButton.isPressed;
        MouseLeftDownThisFrame = mouse.leftButton.wasPressedThisFrame;

        // Mouse.position is an object, needs ReadValue() call to get value
        Vector3 screenSpaceLocation = mouse.position.ReadValue();
        // Convert the mouse's screen position to its equivalent position in the scene
        Vector3 worldSpaceLocation = Camera.main.ScreenToWorldPoint(screenSpaceLocation);

        // Debug code - delete when no longer needed
        if (MouseLeftDownThisFrame)
        {
            Debug.Log($"Mouse clicked at {screenSpaceLocation} on screen.");
            Debug.Log($"Mouse clicked at {worldSpaceLocation} in world.");
        }
    }
}
