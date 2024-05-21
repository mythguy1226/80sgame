using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static event Action<Vector3> detectHitSub;

    // Joycon fields
    public List<Joycon> joycons;
    public Vector3 gyro;
    public int jc_ind = 0;
    public Quaternion orientation;
    public Vector3 joyconCursorPos;
    //private bool shoulderPressed = false;
    public CrosshairBehavior crosshairScript;

    public PauseScreenBehavior pauseScript;

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        gyro = new Vector3(0, 0, 0);
        RecenterCursor();
        ResetRumble();
    }

    // Update is called once per frame
    public void Update()
    {

        // This needs to be ported out - Ed
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Gyro values: x, y, z axis values (in radians per second)
            gyro = j.GetGyro();

            // Update cursor position based on gyroscope values
            joyconCursorPos += new Vector3(gyro.z * 5.0f, gyro.y * 5.0f, 0.0f) ;
            // Clamp the cursor to the screen extents
            joyconCursorPos.x = Mathf.Clamp(joyconCursorPos.x, 0, Screen.width);
            joyconCursorPos.y = Mathf.Clamp(joyconCursorPos.y, 0, Screen.height);

            // Update the cursor position with calculated position
            Mouse.current.WarpCursorPosition(joyconCursorPos);
            crosshairScript.MoveCrosshair(joyconCursorPos);

            // Get right trigger input
            //MouseLeftDownThisFrame = j.GetButtonDown(Joycon.Button.SHOULDER_2);
            
            //shoulderPressed = true;
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                RecenterCursor();
            }

            if ((j.GetButtonDown(Joycon.Button.PLUS) || j.GetButtonDown(Joycon.Button.MINUS)) && !pauseScript.isPaused)
            {
                pauseScript.PauseGame();
            }
            //shoulderPressed = j.GetButtonUp(Joycon.Button.SHOULDER_2);
        }

        
    }

    public void RecenterCursor()
    {
        joyconCursorPos = new Vector3(Screen.width/2, Screen.height / 2, 0);
    }

    public void ResetRumble()
    {
        if (joycons.Count > 0)
        {
            joycons[jc_ind].SetRumble(0, 0, 0);
        }
    }

    public static void PlayerShot(Vector3 position)
    {
        detectHitSub?.Invoke(position);
    }
}
