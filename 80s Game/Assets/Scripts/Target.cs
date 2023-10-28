using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetStates
{
    Moving,
    Fleeing,
    Death
}

public class Target : MonoBehaviour
{
    // Public fields 
    TargetStates currentState = TargetStates.Moving;
    public bool isOnScreen = false;

    // Get needed components for handling target behavior
    KinematicSteer movementControls;

    // Call once upon start of game
    void Awake()
    {
        // Init component references
        movementControls = GetComponent<KinematicSteer>();
    }

    // Update is called once per frame
    void Update()
    {
        // If on-screen check behaviors
        if(isOnScreen)
        {
            // Switch on target states
            // to control decision making
            switch (currentState)
            {
                // Handle all base target movement here
                case TargetStates.Moving:
                    // Enable movement
                    movementControls.canMove = true;
                    break;

                // Handle fleeing behavior here
                case TargetStates.Fleeing:
                    break;

                // Handle death condition here
                case TargetStates.Death:
                    break;
                default:
                    break;
            }

        }
    }
}
