using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsManager : MonoBehaviour
{
    // Public properties
    public int Shots {  get; private set; }
    public int Hits { get; private set; }
    public int Accuracy { get; private set; }

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
        if(Shots > 0)
        {
            // Ratio of hits made to number of shots
            Accuracy = Hits / Shots;
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
        Shots++;
        return ++Hits; 
    }
}
