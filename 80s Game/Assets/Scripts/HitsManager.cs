using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // When the player clicks and the game isn't paused, add a shot
        if (GameManager.Instance.InputManager.MouseLeftDownThisFrame && Time.timeScale > 0)
        {
            if (GameManager.Instance.InputManager.joycons.Count > 0)
            {
                Joycon j = GameManager.Instance.InputManager.joycons[GameManager.Instance.InputManager.jc_ind];
                j.SetRumble(160, 320, 0.6f, 200);
            }
            AddShot();
            //Debug.Log(Shots);
        }
    }

    /// <summary>
    /// Adds a shot and returns the new number of shots
    /// </summary>
    /// <returns></returns>
    public int AddShot()
    {
        return ++Shots;
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
