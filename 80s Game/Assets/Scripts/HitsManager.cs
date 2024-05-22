using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShotInformation
{
    public Vector3 location;
    public int playerInt;
    public ShotInformation(Vector3 l, int p)
    {
        location = l;
        playerInt = p;
    }
}

public class HitsManager : MonoBehaviour
{
    // Public properties
    public int Shots {  get; private set; }
    public int Hits { get; private set; }
    public float Accuracy { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Shots = 0;
        Hits = 0;
        Accuracy = 0;

        InputManager.detectHitSub += AddShot;
    }

    private void OnDisable()
    {
        InputManager.detectHitSub -= AddShot;
    }

    // Update is called once per frame
    void Update()
    {
        // Can't divide by 0
        if (Shots > 0)
        {
            // Ratio of hits made to number of shots
            Accuracy = Hits / (float) Shots;
        }
    }

    /// <summary>
    /// Adds a shot and returns the new number of shots
    /// </summary>
    /// <returns></returns>
    public void AddShot(ShotInformation s)
    {
        bool isGameOngoing = Time.timeScale > 0;
        if (!isGameOngoing)
        {
            return;
        }

        if (JoyconManager.Instance.j.Count > 0)
        {
            Joycon j = JoyconManager.Instance.j[s.playerInt - 1];
            j.SetRumble(160, 320, 0.6f, 200);
        }

        ++Shots;
    }

    /// <summary>
    /// Adds a shot and hit, then returns the new number of hits
    /// </summary>
    /// <returns></returns>
    public int AddHit()
    {
        return ++Hits; 
    }
}
