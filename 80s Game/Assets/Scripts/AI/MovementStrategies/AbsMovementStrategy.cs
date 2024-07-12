using UnityEngine;

public enum MovementStrategy
{
    SimpleFlocking,
    BoidFlocking,
    ReciprocalCollisionAvoidance
}

public static class FlockingData
{
    // Flocking values
    public static float flockSize = 1f;

    public static float separationStrength = 1f;

    public static float cohesionStrength = 1f;

    public static float alignmentStrength = 1f;
}

public abstract class AbsMovementStrategy
{
    public abstract void Initialize();
    public abstract Vector2 Move();

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
}

public class FlockingMovement : AbsMovementStrategy
{
    KinematicSteer movementController;
    public FlockingMovement(KinematicSteer controller)
    {
        movementController = controller;
    }

    public override void Initialize()
    {
        
    }

    public override Vector2 Move()
    {
        Flock();
        UpdateVelocity();
        return UpdatePosition();

    }
    // Method for updating the agent position based on the current velocity
    private Vector2 UpdatePosition()
    {
        float modifiedSpeed = movementController.maxSpeed;
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
    private void UpdateVelocity()
    {
        // Desired velocity for target position
        Vector2 currentPosition = new Vector2(movementController.transform.position.x, movementController.gameObject.transform.position.y);
        Vector2 desiredVelocity = movementController.targetPosition - currentPosition;

        // Steer agent to the wander point
        movementController.currentVelocity += Steer(desiredVelocity.normalized * movementController.maxSpeed);
        movementController.currentVelocity = LimitMagnitude(movementController.currentVelocity, movementController.maxSpeed);
    }

    // Method for returning a steering velocity
    // to reach a desired velocity
    private Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - movementController.currentVelocity;
        steer = LimitMagnitude(steer, movementController.maxForce);

        return steer;
    }

    // Method for applying flocking
    private void Flock()
    {
        // Get the targets on screen
        Target[] targetsOnScreen = GameManager.Instance.TargetManager.ActiveTargets.ToArray();

        // Apply the 3 flocking algorithms to the current velocity
        movementController.currentVelocity += Separate(targetsOnScreen) * FlockingData.separationStrength;
        movementController.currentVelocity += Alignment(targetsOnScreen) * FlockingData.alignmentStrength;
        movementController.currentVelocity += Cohesion(targetsOnScreen) * FlockingData.cohesionStrength;
    }

    // Method used for separation
    private Vector2 Separate(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init direction to default vector
        Vector2 direction = Vector2.zero;
        Vector3 currentPosition = movementController.gameObject.transform.position;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            
            // Continue if the bat isnt on screen
            if (!neighbor.GetComponent<KinematicSteer>().canMove)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(currentPosition, neighbor.transform.position) > FlockingData.flockSize)
                continue;

            // Calculate a vector pointing to the neighbor
            Vector2 difference = currentPosition - neighbor.transform.position;

            // Calculate new direction by dividing by the distance
            direction += difference.normalized / difference.magnitude;
        }

        // Divide the direction by total number of neighbors
        direction /= neighbors.Length;

        // Return desired velocity from steer
        return Steer(direction.normalized * movementController.maxSpeed);
    }

    // Method used for Alignment
    private Vector2 Alignment(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init velocity to default vector
        Vector2 velocity = Vector2.zero;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            // Continue if the bat isnt on screen
            if (!neighbor.GetComponent<KinematicSteer>().canMove)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(movementController.gameObject.transform.position, neighbor.transform.position) > FlockingData.flockSize)
                continue;

            // Add up the velocity of the surrounding neighbors
            velocity += neighbor.GetComponent<KinematicSteer>().currentVelocity;
        }

        // Get the average velocity from the neighbors
        velocity /= neighbors.Length;

        // Return desired velocity from steer
        return Steer(velocity.normalized * movementController.maxSpeed);
    }

    // Method for Cohesion
    private Vector2 Cohesion(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init sum of neighbor positions
        Vector2 sumPositions = Vector2.zero;
        Vector3 currentPosition = movementController.gameObject.transform.position;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            // Continue if the bat isnt on screen
            if (!neighbor.GetComponent<KinematicSteer>().canMove)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(currentPosition, neighbor.transform.position) > FlockingData.flockSize)
                continue;

            // Add position to total
            sumPositions += new Vector2(neighbor.transform.position.x, neighbor.transform.position.y);
        }

        // Calculate the average position and direction towards that average
        Vector2 average = sumPositions / neighbors.Length;
        Vector2 direction = average - new Vector2(currentPosition.x, currentPosition.y);

        // Return desired velocity from steer
        return Steer(direction.normalized * movementController.maxSpeed);
    }
}