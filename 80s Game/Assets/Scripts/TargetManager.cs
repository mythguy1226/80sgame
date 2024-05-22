using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Fields for all targets and spawn locations
    public List<Target> targets;
    public List<Vector3> spawnLocations;

    // Default values
    public int numStuns = 0;
    public int totalStuns = 0;

    // Speed values
    float minSpeed = 3.0f;
    float maxSpeed = 3.5f;

    // Scale values
    float minScale = 2.8f;
    float maxScale = 3.5f;

    public List<Target> ActiveTargets
    {
        get { return targets.FindAll(target => target.FSM.bIsActive); }
    }

    private void Start()
    {
        targets = new List<Target>(GameObject.FindObjectsOfType<Target>());
    }

    public void UpdateTargetParams()
    {
        // Update min and max target speeds
        minSpeed += 0.1f;
        if (minSpeed >= 3.5f)
            minSpeed = 3.5f;

        maxSpeed += 0.1f;
        if (maxSpeed >= 4.0f)
            maxSpeed = 4.0f;
    }

    // Method for spawning a target
    public void SpawnTarget(int targetIndex)
    {
        Debug.Log("Spawning Target " + targetIndex);

        Target target = targets[targetIndex];
        // Get the spawn point randomly and teleport the target to that point
        Vector3 spawnPoint = spawnLocations[Random.Range(0, spawnLocations.Count - 1)];
        target.transform.position = spawnPoint;

        // Start particles on spawn
        if (target.GetComponentInChildren<ParticleSystem>())
        {
            target.GetComponentInChildren<ParticleSystem>().Play();
        }

        // Update target's speed and scaling
        target.FSM.UpdateSpeed(Random.Range(minSpeed, maxSpeed));

        float newScale = Random.Range(minScale, maxScale);
        target.gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);

        // Update on-screen status
        target.FSM.bIsActive = true;
        target.FSM.TransitionToState(BatStateMachine.BatStates.Moving);
        target.GetComponent<AnimationHandler>().ResetAnimation();
        target.GetComponent<PolygonCollider2D>().isTrigger = false;
        target.FSM.SetFleeTimer();
    }

    // Method used for updating values on bat death
    public void OnTargetStun()
    {
        // Update number of stuns
        numStuns++;
        totalStuns++;

        // Allow game mode to update state based on bat stun
        GameManager.Instance.ActiveGameMode.OnTargetStun();
    }
}
