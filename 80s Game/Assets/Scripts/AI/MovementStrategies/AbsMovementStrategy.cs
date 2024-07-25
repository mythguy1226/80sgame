using UnityEngine;

public enum MovementStrategy
{
    SimpleFlocking,
    ObstacleAvoidance
}

public static class FlockingData
{
    // Flocking values
    public static float flockSize = 1f;

    public static float separationStrength = 1.25f;

    public static float cohesionStrength = 0.5f;

    public static float alignmentStrength = 1f;
}

public abstract class AbsMovementStrategy
{
    protected KinematicSteer movementController;

    public AbsMovementStrategy(KinematicSteer controller)
    {
        movementController =  controller;
    }

    public abstract void Initialize();
    public abstract Vector2 Move();
    protected abstract Vector2 Steer(Vector2 desired);

    // Method used for limiting magnitude of a vector
    protected Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        // If magnitude is larger than the max, clamp it to the max
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    // Method for updating the agent position based on the current velocity
    protected Vector2 UpdatePosition()
    {
        float modifiedSpeed = movementController.GetMaxSpeed();
        if (GameManager.Instance.isSlowed)
        {
            modifiedSpeed *= 0.75f;

            // If the stack is 0, modified speed remains unchanged at 0.75
            modifiedSpeed *= 1.0f / ((0.4f * (float)GameManager.Instance.rustedWingsStack) + 1.0f);
        }
        // Calculate moving velocity
        Vector2 finalVelocity = movementController.currentVelocity.normalized * modifiedSpeed;

        // Update position based on final velocity
        return finalVelocity;
    }

    // Method used for updating the current velocity 
    // to steer towards wander point
    protected void UpdateVelocity()
    {
        // Desired velocity for target position
        Vector3 position = movementController.gameObject.transform.position;
        Vector2 currentPosition = new Vector2(position.x, position.y);
        Vector2 desiredVelocity = movementController.targetPosition - currentPosition;

        // Steer agent to the wander point
        movementController.currentVelocity += Steer(desiredVelocity.normalized * movementController.GetMaxSpeed());
        movementController.currentVelocity = LimitMagnitude(movementController.currentVelocity, movementController.GetMaxSpeed());
    }
}
