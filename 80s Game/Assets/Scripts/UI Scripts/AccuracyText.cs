using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccuracyText : MonoBehaviour
{
    public float accuracy;
    public float bonusPoints;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshPro>().text = "Accuracy: " + accuracy.ToString("F0") + "%\n +" + bonusPoints.ToString("F0") + " Pts";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
