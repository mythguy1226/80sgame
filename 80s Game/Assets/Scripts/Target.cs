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
    public TargetStates currentState = TargetStates.Moving;
    public bool isOnScreen = false;

    // Get component for player interactions
    InputManager inputManager;

    // Get needed components for handling target behavior
    KinematicSteer movementControls;

    // Default fields used for resets
    Vector3 spawnPoint;

    // Call once upon start of game
    void Start()
    {
        // Init component references
        movementControls = GetComponent<KinematicSteer>();
        inputManager = GameManager.Instance.InputManager;
        spawnPoint = transform.position;
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

                    // Check for player input coords hitting target
                    Vector3 shotPos = inputManager.MouseWorldPosition;
                    RaycastHit2D hit = Physics2D.Raycast(shotPos, Vector2.zero);
                    if (hit && inputManager.MouseLeftDownThisFrame)
                    {
                        // Check that hit has detected this particular object
                        if(hit.collider.gameObject == gameObject)
                        {
                            currentState = TargetStates.Death;
                        }
                    }
                    break;

                // Handle fleeing behavior here
                case TargetStates.Fleeing:
                    break;

                // Handle death condition here
                case TargetStates.Death:
                    // Reset all target values once in this state
                    Reset();
                    break;
                default:
                    break;
            }

        }
    }

    // Method used for resetting the target
    void Reset()
    {
        // Set target to its default values
        isOnScreen = false;
        transform.position = spawnPoint;
        movementControls.canMove = false;

        // Update the target manager
        GameManager.Instance.TargetManager.numStuns++;

        if (GameManager.Instance.TargetManager.numStuns >= GameManager.Instance.TargetManager.currentRoundSize)
        {
            // Take into account the round cap
            if(GameManager.Instance.TargetManager.currentRound == 4)
            {
                return;
            }

            // Begin the next round and update params
            GameManager.Instance.TargetManager.UpdateRoundParameters();
            GameManager.Instance.TargetManager.StartNextRound();
        }
    }
}
