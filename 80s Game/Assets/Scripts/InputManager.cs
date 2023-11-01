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

    private List<Joycon> joycons;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;
    public Vector3 joyconCursorPos;

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
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
            Debug.Log(gyro);

            // Accel values:  x, y, z axis values (in Gs)
            //accel = j.GetAccel();
            //Debug.Log(accel);

            // Update cursor position based on gyroscope
            joyconCursorPos += new Vector3(gyro.z, gyro.y, 0.0f);
            Mouse.current.WarpCursorPosition(joyconCursorPos);



        }
    }
}
