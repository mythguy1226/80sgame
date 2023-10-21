using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicSteer : MonoBehaviour
{
    // Public fields
    public Vector3 targetPosition;
    public float maxSpeed;
    public float targetRadius;
    public bool canMove;

    // Private fields for calculations
    Vector3 currentVelocity;

    // Update is called once per frame
    void Update()
    {
        // Only steer if agent is currently moveable
        if (canMove)
            Steer();
    }

    public void Steer()
    {
        // Get the direction towards target position
        Vector2 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // Stop moving if at destination
        if (distance <= targetRadius)
        {
            // Find new position once at destination
            SetWanderPosition();
            return;
        }

        // Calculate and set the velocity
        Vector2 velocity = direction.normalized * (distance / targetRadius);
        currentVelocity = velocity;

        // Clamp the velocity to the max speed
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        // Incorporate flocking behavior in final velocity
        velocity += Separate(TargetManager.targets);
        //velocity += Alignment(TargetManager.targets);
        //velocity += Cohesion(TargetManager.targets);

        // Get new position
        float newPosX = transform.position.x + velocity.x * Time.deltaTime;
        float newPosY = transform.position.y + velocity.y * Time.deltaTime;

        // Update target position
        transform.position = new Vector2(newPosX, newPosY);
    }

    // Method used for Wandering
    public void SetWanderPosition()
    {
        // Get max heigh and width values from screen
        float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
        float maxWidth = maxHeight * Screen.width / Screen.height;

        // Get a random range for x and y levels
        float newPosX = Random.Range(-maxWidth, maxWidth);
        float newPosY = Random.Range(-maxHeight/2, maxHeight/2);

        // Set the new position
        targetPosition = new Vector3(newPosX, newPosY, transform.position.z);

    }

    // Method used for separation
    public Vector2 Separate(Target[] neighbors)
    {
        // Initialize values to defaults
        Vector3 desiredVelocity = Vector3.zero;
        int count = 0;

        // Check each neighbor
        foreach (Target neighbor in neighbors)
        {
            // Get the neighbor position and the distance between the object and it's neighbor
            Vector3 neighborPos = neighbor.transform.position;
            float distance = Vector3.Distance(transform.position, neighborPos);

            // If the distance less than the desired seperation and it's greater than zero, separate
            if (distance < (transform.localScale.x * 2) + 4 && distance > 0)
            {
                // Add the normalized vectors between the object position and each neighbor's position to the desired velocity
                Vector3 steeringForce = transform.position - neighborPos;
                steeringForce.Normalize();
                desiredVelocity += steeringForce;
                count++;
            }
        }

        // Only separate if there were neighbors around you
        if (count > 0)
        {
            // Divide the summed up velocities by number of neighbors
            desiredVelocity /= count;

            // Magnitize the velocity to max speed
            desiredVelocity = desiredVelocity.normalized * maxSpeed;

            // Subtract current velocity from the desired velocity
            desiredVelocity -= currentVelocity;
        }

        // Return steering force
        return desiredVelocity * 0.1f;
    }

    // Method used for Alignment
    public Vector2 Alignment(Target[] neighbors)
    {
        // Initialize to default zero vector
        Vector3 desiredVelocity = Vector3.zero;

        // Get all directions for all neighbors within a radius of 7
        foreach (Target neighbor in neighbors)
        {
            if (Vector3.Distance(transform.position, neighbor.transform.position) < 7)
            {
                desiredVelocity += (neighbor.GetComponent<KinematicSteer>().targetPosition - neighbor.transform.position);
            }
        }

        // Magnitize to max speed and subtract current velocity from it
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        desiredVelocity -= currentVelocity;

        // Return steering force
        return desiredVelocity * 0.2f;
    }

    // Method for Cohesion
    public Vector2 Cohesion(Target[] neighbors)
    {
        Vector3 centroid = Vector3.zero;
        int count = 0;

        // Get the sum of all points of neighbors within a radius of 7
        foreach (Target neighbor in neighbors)
        {
            if (Vector3.Distance(transform.position, neighbor.transform.position) < 7)
            {
                centroid += neighbor.transform.position;
                count++;
            }
        }

        // Divide by number of neighbors to get the average point
        centroid /= count;

        // Seek that average point AKA the Centroid
        return centroid * 0.2f;
    }
}
