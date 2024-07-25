using UnityEngine;

public class FlockingMovement : AbsMovementStrategy
{

    public FlockingMovement(KinematicSteer controller) : base(controller)
    {

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

    // Method for returning a steering velocity
    // to reach a desired velocity
    protected override Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - movementController.currentVelocity;
        steer = LimitMagnitude(steer, movementController.GetMaxForce());

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
        if (neighbors.Length <= 1)
            return Vector2.zero;

        // Init direction to default vector
        Vector2 direction = Vector2.zero;
        Vector3 currentPosition = movementController.gameObject.transform.position;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            float distance = Vector3.Distance(currentPosition, neighbor.transform.position);

            // Continue if the bat isnt on screen
            if (!neighbor.GetComponent<KinematicSteer>().canMove)
                continue;

            // Continue if the bat isnt in the flock radius
            if (distance > FlockingData.flockSize)
                continue;
            
            if (distance == 0)
            {
                // Would cause division by zero error instead we give it a small value
                distance = 0.01f;
            }

            Debug.DrawLine(currentPosition, neighbor.transform.position, Color.magenta);

            // Calculate a vector pointing to the neighbor
            Vector2 difference = currentPosition - neighbor.transform.position;
            float inverseMagnitude = 1.0f / distance;
            // Calculate new direction by dividing by the distance
            direction += Vector2.Scale(difference.normalized, new Vector2(inverseMagnitude, inverseMagnitude));
        }

        // Divide the direction by total number of neighbors
        direction /= neighbors.Length;
        //Debug.Log(direction.normalized);

        // Return desired velocity from steer
        Debug.DrawLine(currentPosition, new Vector2(currentPosition.x, currentPosition.y) + direction.normalized, Color.green);
        return Steer(direction.normalized * movementController.GetMaxSpeed());
    }

    // Method used for Alignment
    private Vector2 Alignment(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length <= 1)
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
            if (Vector3.Distance(movementController.GetPosition(), neighbor.transform.position) > FlockingData.flockSize)
                continue;

            // Add up the velocity of the surrounding neighbors
            velocity += neighbor.GetComponent<KinematicSteer>().currentVelocity;
        }

        // Get the average velocity from the neighbors
        velocity /= neighbors.Length;
        Debug.DrawLine(movementController.GetPosition(), movementController.GetPosition() + velocity, Color.red);
        // Return desired velocity from steer
        return Steer(velocity.normalized * movementController.GetMaxSpeed());
    }

    // Method for Cohesion
    private Vector2 Cohesion(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length <= 1)
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
        Debug.DrawLine(currentPosition, new Vector2(currentPosition.x, currentPosition.y) + direction.normalized, Color.cyan);
        // Return desired velocity from steer
        return Steer(direction.normalized * movementController.GetMaxSpeed());
    }
}