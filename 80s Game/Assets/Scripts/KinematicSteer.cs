using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KinematicSteer : MonoBehaviour
{
    // Public fields
    public Vector2 targetPosition;
    public float targetRadius;
    public bool canMove;
    public bool isFleeing;

    // Maximums
    [Range(0, 10)]
    public float maxSpeed = 3f;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    // Private fields for calculations
    public Vector2 currentVelocity;
    private Vector2 originalScale;

    // Get the sprite renderer
    SpriteRenderer spriteRenderer;
    Rigidbody2D _rb;

    // Flocking values
    [Range(0.1f, 3.0f)]
    public float flockSize = 1f;

    [Range(0.0f, 5.0f)]
    public float separationStrength = 1f;

    [Range(0.0f, 5.0f)]
    public float cohesionStrength = 1f;

    [Range(0.0f, 5.0f)]
    public float alignmentStrength = 1f;

    // Ran at beginning of game
    void Start()
    {
        // Init components
        spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        // Set the first wander position
        SetWanderPosition();

        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Only steer if agent is currently moveable
        if (canMove)
        {
            // Check if target reaches destination in wander state
            if (IsAtDestination() && !isFleeing)
            {
                // Find new position once at destination
                SetWanderPosition();
                return;
            }

            // Agents flock together and also steer
            // towards a wander location in the level
            Flock();
            UpdateVelocity();
            UpdatePosition();
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }

        // Set direction to face
        if (currentVelocity.x > 0.0f)
            transform.localScale = new Vector2(-originalScale.x, originalScale.y);
        else
            transform.localScale = originalScale;

    }

    // Method for updating the agent position based on the current velocity
    public void UpdatePosition()
    {
        float modifiedSpeed = maxSpeed;
        if (GameManager.Instance.isSlowed)
        {
            modifiedSpeed *= 0.75f;
        }
        // Calculate moving velocity
        Vector2 finalVelocity = currentVelocity.normalized * modifiedSpeed;

        // Update position based on final velocity
        //transform.position += new Vector3(finalVelocity.x, finalVelocity.y, 0.0f) * Time.deltaTime;
        _rb.velocity = finalVelocity;
    }

    // Method used for updating the current velocity 
    // to steer towards wander point
    public void UpdateVelocity()
    {
        // Desired velocity for target position
        Vector2 desiredVelocity = targetPosition - new Vector2(transform.position.x, transform.position.y);

        // Steer agent to the wander point
        currentVelocity += Steer(desiredVelocity.normalized * maxSpeed);
        currentVelocity = LimitMagnitude(currentVelocity, maxSpeed);
    }

    // Method for returning a steering velocity
    // to reach a desired velocity
    public Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - currentVelocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    // Method for setting the target position
    public void SetTargetPosition(Vector2 newTarget)
    {
        // Set the position
        targetPosition = newTarget;
    }

    // Method used for Wandering
    public void SetWanderPosition()
    {
        // Get max heigh and width values from screen
        float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
        float maxWidth = maxHeight * (Screen.width / Screen.height);

        // Get a random range for x and y levels
        float newPosX = Random.Range((-maxWidth * 2) + spriteRenderer.size.x, (maxWidth * 2) - spriteRenderer.size.x);
        float newPosY = Random.Range(-maxHeight + spriteRenderer.size.y, maxHeight - spriteRenderer.size.y);

        // Set the new position
        SetTargetPosition(new Vector2(newPosX, newPosY));
    }

    // Method for checking if target has arrived at destination
    public bool IsAtDestination()
    {
        // Get the direction towards target position
        Vector2 direction = targetPosition - new Vector2(transform.position.x, transform.position.y);
        float distance = direction.magnitude;

        // Return result
        return distance <= targetRadius;
    }

    public bool IsInsideScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1;
    }

    // Method for applying flocking
    public void Flock()
    {
        // Get the targets on screen
        Target[] targetsOnScreen = GameManager.Instance.TargetManager.ActiveTargets.ToArray();

        // Apply the 3 flocking algorithms to the current velocity
        currentVelocity += Separate(targetsOnScreen) * separationStrength;
        currentVelocity += Alignment(targetsOnScreen) * alignmentStrength;
        currentVelocity += Cohesion(targetsOnScreen) * cohesionStrength;
    }

    // Method used for separation
    public Vector2 Separate(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init direction to default vector
        Vector2 direction = Vector2.zero;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            // Continue if the bat isnt on screen
            if (!neighbor.FSM.bIsActive)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(transform.position, neighbor.transform.position) > flockSize)
                continue;

            // Calculate a vector pointing to the neighbor
            Vector2 difference = transform.position - neighbor.transform.position;

            // Calculate new direction by dividing by the distance
            direction += difference.normalized / difference.magnitude;
        }

        // Divide the direction by total number of neighbors
        direction /= neighbors.Length;

        // Return desired velocity from steer
        return Steer(direction.normalized * maxSpeed);
    }

    // Method used for Alignment
    public Vector2 Alignment(Target[] neighbors)
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
            if (!neighbor.FSM.bIsActive)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(transform.position, neighbor.transform.position) > flockSize)
                continue;

            // Add up the velocity of the surrounding neighbors
            velocity += neighbor.GetComponent<KinematicSteer>().currentVelocity;
        }

        // Get the average velocity from the neighbors
        velocity /= neighbors.Length;

        // Return desired velocity from steer
        return Steer(velocity.normalized * maxSpeed);
    }

    // Method for Cohesion
    public Vector2 Cohesion(Target[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init sum of neighbor positions
        Vector2 sumPositions = Vector2.zero;

        // Iterate through each neighbor
        foreach (Target neighbor in neighbors)
        {
            // Continue if the bat isnt on screen
            if (!neighbor.FSM.bIsActive)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(transform.position, neighbor.transform.position) > flockSize)
                continue;

            // Add position to total
            sumPositions += new Vector2(neighbor.transform.position.x, neighbor.transform.position.y);
        }

        // Calculate the average position and direction towards that average
        Vector2 average = sumPositions / neighbors.Length;
        Vector2 direction = average - new Vector2(transform.position.x, transform.position.y);

        // Return desired velocity from steer
        return Steer(direction.normalized * maxSpeed);
    }

    // Method used for limiting magnitude of a vector
    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        // If magnitude is larger than the max, clamp it to the max
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    // Method for detecting collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<KinematicSteer>())
        {
            // Get max height and width values from screen
            float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
            float maxWidth = maxHeight * (Screen.width / Screen.height);

            if (!isFleeing)
                SetWanderPosition();
            else
                targetPosition.x = UnityEngine.Random.Range((-maxWidth * 2) + spriteRenderer.size.x, (maxWidth * 2) - spriteRenderer.size.x);
        }
    }
}
