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
        target.FSM.Spawn();
        target.GetComponent<AnimationHandler>().ResetAnimation();
        target.GetComponent<PolygonCollider2D>().isTrigger = false;
        
    }

    // Method used for updating values on bat death
    public void OnTargetReset()
    {
        // Update number of stuns
        numStuns++;
        totalStuns++;

        // Allow game mode to update state based on bat stun
        GameManager.Instance.ActiveGameMode.OnTargetReset();
    }

    /// <summary>
    /// Templated method that finds the next target
    /// containing the templated class type
    /// </summary>
    /// <typeparam name="T">Templated class type</typeparam>
    /// <returns>Index of available bat</returns>
    public int GetNextAvailableTargetOfType<T>()
    {
        // Iterate through the targets until you
        // find one that isn't already on screen
        for (int i = 0; i < targets.Count; i++)
        {
            // Keep iterating if already active
            if (targets[i].FSM.bIsActive)
                continue;

            // Keep iterating if target lacks component of templated class type
            T comp = targets[i].GetComponent<T>();
            if (comp == null)
                continue;

            // Return index of bat
            return i;
        }

        return -1;
    }
}
