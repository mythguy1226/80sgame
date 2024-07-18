using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class MovementStrategyFactory
{
    static List<MovementStrategy> movementTypes = Enum.GetValues(typeof(MovementStrategy)).Cast<MovementStrategy>().ToList();
    public static AbsMovementStrategy MakeMovementStrategy(MovementStrategy type, KinematicSteer controller)
    {
        switch (type)
        {
            case MovementStrategy.ObstacleAvoidance:
                return new VelocityObstacleAvoidance(controller);
            default:
                return new FlockingMovement(controller);
        }
    }

    public static AbsMovementStrategy MakeRandomMovementStrategy(KinematicSteer controller) {
        MovementStrategy movementStrategy = GetRandomMovementStrategy();
        return MakeMovementStrategy(movementStrategy, controller);
    }

    private static MovementStrategy GetRandomMovementStrategy()
    {
        return movementTypes[Random.Range(0, movementTypes.Count)];
    }
}