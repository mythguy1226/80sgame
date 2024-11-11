using UnityEngine;

public class VelocityObstacleAvoidance : AbsMovementStrategy
{
    public VelocityObstacleAvoidance(KinematicSteer controller) : base(controller) {

    }

    public override Vector2 Move()
    {
        UpdateVelocity();
        AvoidObstacles();
        if (DistanceOverride())
        {
            
            Vector3 position = movementController.gameObject.transform.position;
            Vector2 currentPosition = new Vector2(position.x, position.y);
            movementController.currentVelocity = (movementController.targetPosition - currentPosition).normalized * movementController.GetMaxSpeed();
            Debug.DrawLine(currentPosition, currentPosition + movementController.currentVelocity);
        }
        return UpdatePosition();
        
    }

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    private void AvoidObstacles()
    {
        Target[] targetsOnScreen = GameManager.Instance.TargetManager.ActiveTargets.ToArray();
        Declump(targetsOnScreen);
        EvaluateVelocityObstacles(targetsOnScreen);
    }

    private void Declump(Target[] targetsOnScreen)
    {
        float declumpDistance = 1.5f;
        if (targetsOnScreen.Length <= 1)
        {
            return;
        }
        Vector2 finalVector = movementController.currentVelocity;
        Vector2 position = movementController.GetPosition();
        foreach (Target target in targetsOnScreen) {
            KinematicSteer otherMovement = target.GetMovementController();
            Vector2 otherPosition = otherMovement.transform.position;
            Vector2 directionToOther = otherPosition - position;

            // If the target is too far away, continue
            if (directionToOther.magnitude > declumpDistance)
            {
                continue;
            }

            finalVector += Steer(-directionToOther);
        }

    }

    private void EvaluateVelocityObstacles(Target[] targetsOnScreen)
    {
        
        float maxDistance = 2.0f;
        
        // Quit early if there is nothing to do.
        if (targetsOnScreen.Length <= 1) {
            return;
        }

        Vector2 currentVelocityCache = movementController.currentVelocity;
        Vector2 normalizedCurrentVelocity = currentVelocityCache.normalized;
        Vector2 finalVector = movementController.currentVelocity;
        Vector2 position = movementController.GetPosition();
        Vector2 normalizedPosPlusVel = (position + currentVelocityCache).normalized;

        // Cast 4 whiskers
        // Whisker 1 is the reflex of whisker 3 along the central velocity line
        // Whisker 2 is the reflex of whisker 4 along the central velocity line
        Vector2 whisker1 = CreateWhisker(currentVelocityCache, 75, false);
        Vector2 posWhis1 = position + whisker1;
        Vector2 whisker2 = CreateWhisker(currentVelocityCache, 32, false);
        Vector2 posWhis2 = position + whisker2;
        Vector2 whisker3 = CreateWhisker(currentVelocityCache, 75, true);
        Vector2 posWhis3 = position + whisker3;
        Vector2 whisker4 = CreateWhisker(currentVelocityCache, 32, true);
        Vector2 posWhis4 = position + whisker4;

        Vector2 whisker1Steer = Steer(-whisker1 * movementController.GetMaxSpeed());
        Vector2 whisker2Steer = Steer(-whisker2 * movementController.GetMaxSpeed());
        Vector2 whisker3Steer = Steer(-whisker3 * movementController.GetMaxSpeed());
        Vector2 whisker4Steer = Steer(-whisker4 * movementController.GetMaxSpeed());

        Debug.DrawLine(position, posWhis1);
        Debug.DrawLine(position, posWhis2);
        Debug.DrawLine(position, posWhis3);
        Debug.DrawLine(position, posWhis4);
        Debug.DrawLine(position, position + currentVelocityCache.normalized);
        
        foreach (Target target in targetsOnScreen) {
            
            KinematicSteer otherMovement = target.GetMovementController();
            Vector2 otherPosition = otherMovement.transform.position;
            // If the target is behind me, continue
            Vector2 directionToOther = otherPosition - position;

            // If the target is too far away, continue
            if (directionToOther.magnitude > maxDistance)
            {
                continue;
            }

            if (!otherMovement.canMove)
                continue;

            if (Vector2.Dot(directionToOther.normalized, normalizedCurrentVelocity) < 0) {
                continue;
            }

            Vector2 impact = Vector2.zero;
            float distanceWhisker1 = Vector2.Distance(otherPosition, posWhis1);
            float distanceWhisker2 = Vector2.Distance(otherPosition, posWhis2);
            float distanceWhisker3 = Vector2.Distance(otherPosition, posWhis3);
            float distanceWhisker4 = Vector2.Distance(otherPosition, posWhis4);
            float currentVelocityDot = Vector2.Dot(otherPosition, normalizedCurrentVelocity);
            
            // Calculate the influence of each whisker
            impact += whisker1Steer * (1 - (distanceWhisker1 / maxDistance));
            impact += whisker2Steer * (1 - (distanceWhisker2 / maxDistance));
            impact += whisker3Steer * (1 - (distanceWhisker3 / maxDistance));
            impact += whisker4Steer * (1 - (distanceWhisker4 / maxDistance));

            finalVector += impact;
            
        }
        movementController.currentVelocity += finalVector;
    }

    // Method for returning a steering velocity
    // to reach a desired velocity
    protected override Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - movementController.currentVelocity;
        steer = LimitMagnitude(steer, movementController.GetMaxForce());

        return steer;
    }

    private Vector2 CreateWhisker(Vector3 velocity, float angle, bool counterClockWise) {
        Vector3 rotationAngle = Vector3.forward;
        if (counterClockWise) {
            rotationAngle = Vector3.back;
        }
        Vector2 whisker = Quaternion.AngleAxis(angle, rotationAngle) * velocity.normalized;
        return whisker.normalized;
    }

    private bool DistanceOverride()
    {
        float distance = Vector2.Distance(movementController.targetPosition, movementController.gameObject.transform.position);
        return distance > movementController.maxDistance;
    }
}