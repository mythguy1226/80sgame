using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicSteer : MonoBehaviour
{
    // Public fields
    public Vector2 targetPosition;
    public float targetRadius;
    public bool canMove;
    public bool isWandering;
    public bool isFleeing;
    // Maximums
    [Range(0, 10)]
    public float maxSpeed = 3f;


    // Private fields for calculations
    public Vector2 currentVelocity;
    private Vector2 originalScale;

    // Get the sprite renderer
    SpriteRenderer spriteRenderer;
    Rigidbody2D _rb;

    AbsMovementStrategy movementStrategy;


    public void Initialize()
    {
        movementStrategy = MovementStrategyFactory.MakeRandomMovementStrategy(this);
        SetWanderPosition();
        _rb.velocity = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
    }

    // Ran at beginning of game
    void Start()
    {
        // Init components
        spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        

        // Set the first wander position
        Initialize();

        originalScale = transform.localScale;
        isWandering = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Only steer if agent is currently moveable
        if (canMove)
        {
            // Check if target reaches destination in wander state
            if (IsAtDestination() && !isFleeing && isWandering)
            {
                // Find new position once at destination
                SetWanderPosition();
                return;
            }

            // Agents flock together and also steer
            // towards a wander location in the level
            _rb.velocity = movementStrategy.Move();
            if (GameManager.Instance.ActiveGameMode.isInDebugMode())
            {
                Debug.DrawLine(transform.position, targetPosition, Color.red);
            }


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

    // Method for setting the target position
    public void SetTargetPosition(Vector2 newTarget)
    {
        // Set the position
        targetPosition = newTarget;
    }

    // Method used for Wandering
    private void SetWanderPosition()
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
    private bool IsAtDestination()
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

    public Vector2 GetPosition()
    {
        
        return new Vector2(transform.position.x, transform.position.y);
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetMaxForce()
    {
        return GameManager.Instance.maxForce;
    }

}
