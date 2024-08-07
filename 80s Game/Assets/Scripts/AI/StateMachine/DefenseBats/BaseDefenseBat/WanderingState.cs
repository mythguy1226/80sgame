using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingState : AbsBaseState<DefenseBatStateMachine.DefenseBatStates>
{
    // Get components
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;

    // Constructor with call to base state class
    public WanderingState() : base(DefenseBatStateMachine.DefenseBatStates.Wandering)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        OwnerFSM.gameObject.GetComponent<Target>().bIsStunned = false;
    }

    /*
	USAGE: Handler for state exit
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void ExitState()
    {
        // Any clean up needed from this state will go here
    }

    /*
	USAGE: Handler for state update ticks
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void UpdateState()
    {
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        if (FSM == null)
            return;

        _MovementControls = FSM.MovementControls;
        _SpriteRenderer = FSM.SpriteRenderer;

        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        // Enable movement
        _MovementControls.isFleeing = false;
        _MovementControls.canMove = true;
        _MovementControls.isWandering = true;

        // Update pursue timer
        FSM.pursueTimer -= Time.deltaTime;

    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: DefenseBatStates, state to transition to given current game conditions
	*/
    public override DefenseBatStateMachine.DefenseBatStates GetNextState()
    {
        
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        // When timer is up, set target to flee
        if(FSM != null)
        {
            if (FSM.pursueTimer <= 0.0f)
            {
                // Try to find a latch position to target
                LatchPoint closestLatchPoint = FSM.GetClosestLatchPoint();

                // Check whether a latch was available
                if(closestLatchPoint == null)
                {
                    // Keep wandering since no latch was available
                    FSM.SetPursueTimer();
                    return DefenseBatStateMachine.DefenseBatStates.Wandering;
                }
                else // Set target attack location to the latch position
                {
                    FSM.targetLatch = closestLatchPoint;
                    FSM.targetLatch.isAvailable = false;
                }

                // Set new target position to be the closest attack location
                _MovementControls.SetTargetPosition(FSM.targetLatch.transform.position);

                return DefenseBatStateMachine.DefenseBatStates.Pursuing;
            }
        }
        
        return DefenseBatStateMachine.DefenseBatStates.Wandering;
    }
}
