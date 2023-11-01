using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Public properties
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

    public Vector3 MouseWorldPosition
    {
        get;
        private set;
    }

    // Joycon fields
    private List<Joycon> joycons;
    public Vector3 gyro;
    public int jc_ind = 0;
    public Quaternion orientation;
    public Vector3 joyconCursorPos;

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        gyro = new Vector3(0, 0, 0);
        joyconCursorPos = new Vector3(0, 0, 0);
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
        MouseWorldPosition = Camera.main.ScreenToWorldPoint(screenSpaceLocation);

        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Gyro values: x, y, z axis values (in radians per second)
            gyro = j.GetGyro();

            // Update cursor position based on gyroscope values
            joyconCursorPos += new Vector3(gyro.z, gyro.y, 0.0f);

            // Clamp the cursor to the screen extents
            joyconCursorPos.x = Mathf.Clamp(joyconCursorPos.x, 0, Screen.width);
            joyconCursorPos.y = Mathf.Clamp(joyconCursorPos.y, 0, Screen.height);

            // Update the cursor position with calculated position
            Mouse.current.WarpCursorPosition(joyconCursorPos);

            // Get right trigger input
            MouseLeftDownThisFrame = j.GetButtonDown(Joycon.Button.SHOULDER_2);

        }
    }
}
