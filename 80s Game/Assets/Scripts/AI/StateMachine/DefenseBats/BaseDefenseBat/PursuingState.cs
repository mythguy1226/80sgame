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
        // Get state machine and needed components
        DefenseBatStateMachine FSM = (DefenseBatStateMachine)OwnerFSM;

        if (FSM == null)
            return;

        _MovementControls = FSM.MovementControls;

        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        // Enable movement and disable wandering
        _MovementControls.isFleeing = false;
        _MovementControls.canMove = true;
        _MovementControls.isWandering = false;
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
            if(Vector3.Distance(FSM.targetAttackLocation, FSM.transform.position) <= 1.0f)
            {
                return DefenseBatStateMachine.DefenseBatStates.Attacking;
            }
        }
        return DefenseBatStateMachine.DefenseBatStates.Pursuing;
    }
}
