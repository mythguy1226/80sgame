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

            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = Camera.main.orthographicSize * 2;

            joyconCursorPos.x = Mathf.Clamp(joyconCursorPos.x, -cameraHeight * screenAspect, cameraHeight * screenAspect);
            joyconCursorPos.y = Mathf.Clamp(joyconCursorPos.y, -cameraHeight, cameraHeight);


            Mouse.current.WarpCursorPosition(joyconCursorPos);

            // Get orientation from joycon
            orientation = j.GetVector(); // <- Misleading method name, this returns a quaternion

            // Get forward and rightward vectors from orientation
            //Vector3 curForward = orientation * Vector3.forward;
            //Vector3 curRightward = orientation * Vector3.right;
            //Debug.Log(curForward);
            //Debug.Log(curRightward);

        }
    }
}
