using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuingState : AbsBaseState<DefenseBatStateMachine.DefenseBatStates>
{
    // Get components
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;

    // Constructor with call to base state class
    public PursuingState() : base(DefenseBatStateMachine.DefenseBatStates.Pursuing)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        // Get state machine and needed components
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        if (FSM == null)
            return;

        _MovementControls = FSM.MovementControls;

        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        // Prevent movement until all preparation is done
        _MovementControls.canMove = false;

        // Scale bat speed with its pursue speed
        Target targ = FSM.GetComponent<Target>();
        targ.UpdateSpeed(targ.MovementSpeed * FSM.pursueSpeedScale);

        // Disable circle collision with other colliders
        FSM.GetComponent<CircleCollider2D>().isTrigger = true;
        FSM.GetComponent<PolygonCollider2D>().isTrigger = true;

        // Call logic for beginning pursuit
        FSM.BeginPursue();
    }

    /*
	USAGE: Handler for state exit
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void ExitState()
    {
        // Get state machine and needed components
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        if (FSM == null)
            return;

        // Any clean up needed from this state will go here
        FSM.GetComponent<CircleCollider2D>().isTrigger = false;
        FSM.GetComponent<PolygonCollider2D>().isTrigger = false;
    }

    /*
	USAGE: Handler for state update ticks
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void UpdateState()
    {
        // Get state machine and needed components
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        if (FSM == null)
            return;

        // Return if not able to pursue
        if(!FSM.bCanPursue)
            return;

        _MovementControls = FSM.MovementControls;

        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        // Enable movement and disable wandering
        _MovementControls.isFleeing = false;
        _MovementControls.canMove = true;
        _MovementControls.isWandering = false;

        // Ensure bat is always pursuing the set target attack location
        if(_MovementControls.targetPosition != (Vector2)FSM.targetLatch.transform.position)
            _MovementControls.SetTargetPosition(FSM.targetLatch.transform.position);
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
            if(Vector3.Distance(FSM.targetLatch.transform.position, FSM.transform.position) <= 1.0f)
            {
                FSM.targetLatch.LatchTarget(FSM.GetComponent<Target>());
                return DefenseBatStateMachine.DefenseBatStates.Attacking;
            }
        }
        return DefenseBatStateMachine.DefenseBatStates.Pursuing;
    }
}
