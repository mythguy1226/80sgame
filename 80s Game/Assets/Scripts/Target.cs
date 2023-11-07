using System;
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
    public int pointValue = 1000;

    // Fleeing fields
    public float timeUntilFlee = 5.0f;
    float fleeTimer = 0.0f;
    public Vector2 fleeLocation;

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

        // Init flee timer
        fleeTimer = timeUntilFlee;
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
                    movementControls.isFleeing = false;
                    movementControls.canMove = true;

                    // Update flee timer
                    fleeTimer -= Time.deltaTime;
                    if(fleeTimer <= 0.0f) // When timer is up, set target to flee
                    {
                        currentState = TargetStates.Fleeing;
                        movementControls.SetTargetPosition(fleeLocation);
                    }

                    // Check for stun
                    DetectStun();
                    break;

                // Handle fleeing behavior here
                case TargetStates.Fleeing:
                    movementControls.isFleeing = true;

                    // Check for stun
                    DetectStun();

                    // Check if destination is reached
                    if (movementControls.IsAtDestination())
                        Reset();
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

    // Method used for setting target flee timer publicly
    public void SetFleeTimer()
    {
        fleeTimer = timeUntilFlee;
    }

    // Method for checking if target has been stunned
    void DetectStun()
    {
        // Check for player input coords hitting target
        Vector3 shotPos = inputManager.MouseWorldPosition;
        RaycastHit2D hit = Physics2D.Raycast(shotPos, Vector2.zero);
        if (hit && inputManager.MouseLeftDownThisFrame && Time.timeScale > 0) // Check time scale so bats cant be harmed while game is paused
        {
            // Check that hit has detected this particular object
            if (hit.collider.gameObject == gameObject)
            {
                currentState = TargetStates.Death;
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

        // Choose new wander position to be used on respawn
        movementControls.SetWanderPosition();

        GameManager gameManager = GameManager.Instance;

        // Update the target manager
        gameManager.TargetManager.numStuns++;

        // Add points if target didn't flee
        if(currentState != TargetStates.Fleeing)
            gameManager.PointsManager.AddRoundPoints(pointValue);
        
        // Add a successful hit
        gameManager.HitsManager.AddHit();

        if (GameManager.Instance.TargetManager.numStuns >= GameManager.Instance.TargetManager.currentRoundSize)
        {
            // Only add points if target didn't flee
            if (currentState != TargetStates.Fleeing)
            {
                // A little verbose, but can be improved later on
                PointsManager pointsManager = GameManager.Instance.PointsManager;
                pointsManager.AddBonusPoints(GameManager.Instance.HitsManager.Accuracy);
                pointsManager.AddTotal();
            }

            // Take into account the round cap
            if (GameManager.Instance.TargetManager.currentRound == 10)
            {
                return;
            }

            // Begin the next round and update params
            GameManager.Instance.TargetManager.UpdateRoundParameters();
            GameManager.Instance.TargetManager.StartNextRound();
        }
        else
        {
            // Try spawning another target
            GameManager.Instance.TargetManager.SpawnMoreTargets();
        }
    }
}
