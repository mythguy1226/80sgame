using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreInitials : MonoBehaviour
{
    //Int fields to keep track of ASCII value of initial
    int initialOne = 65;
    int initialTwo = 65;
    int initialThree = 65;

    //Text fields for each of the initials in the UI
    public TMP_Text initialOneText;
    public TMP_Text initialTwoText;
    public TMP_Text initialThreeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //cast ints to a char
        char tempCharOne = (char)initialOne;
        char tempCharTwo = (char)initialTwo;
        char tempCharThree = (char)initialThree;

        //Update text initials in UI
        initialOneText.SetText(tempCharOne.ToString());
        initialTwoText.SetText(tempCharTwo.ToString());
        initialThreeText.SetText(tempCharThree.ToString());
    }

    //Increase ASCII value of initial
    //Clamped so initials can only be A-Z
    public void IncreaseInitial(int initialNum)
    {
        switch(initialNum)
        {
            //Used for first initial
            case 1:
                initialOne = Mathf.Clamp(++initialOne, 65, 90);
                break;
            //Used for second initial
            case 2:
                initialTwo = Mathf.Clamp(++initialTwo, 65, 90);
                break;
            //Used for third initial
            case 3:
                initialThree = Mathf.Clamp(++initialThree, 65, 90);
                break;
            default:
                break;
        }
            
    }

    //Decrease ASCII value of initial
    //Clamped so initials can only be A-Z
    public void DecreaseInitial(int initialNum)
    {
        switch (initialNum)
        {
            //Used for first initial
            case 1:
                initialOne = Mathf.Clamp(--initialOne, 65, 90);
                break;
            //Used for second initial
            case 2:
                initialTwo = Mathf.Clamp(--initialTwo, 65, 90);
                break;
            //Used for third initial
            case 3:
                initialThree = Mathf.Clamp(--initialThree, 65, 90);
                break;
            default:
                break;
        }
    }
}
