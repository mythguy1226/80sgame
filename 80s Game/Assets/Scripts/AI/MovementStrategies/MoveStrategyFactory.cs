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
            default:
                return new FlockingMovement(controller);
        }
    }

    public static MovementStrategy GetRandomMovementStrategy()
    {
        return movementTypes[Random.Range(0, movementTypes.Count)];
    }
}