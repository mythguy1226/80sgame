using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static event Action<ShotInformation> detectHitSub;

    // Joycon fields
    public List<Joycon> joycons = new List<Joycon>();
    public Vector3 gyro;
    public int jc_ind = 0;
    public Quaternion orientation;
    public Vector3 joyconCursorPos;
    //private bool shoulderPressed = false;
    public CrosshairBehavior crosshairScript;
    public PauseScreenBehavior pauseScript;
    public float sensitivity = 5.0f;
    // Update is called once per frame
    public void Update()
    {        
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

    public static void PlayerShot(ShotInformation s)
    {
        if (s.isRadiusCheck)
        {
            AchievementManager.ResetOverchargedByPlayer(s.player.Order);
        }
        detectHitSub?.Invoke(s);
    }
}
