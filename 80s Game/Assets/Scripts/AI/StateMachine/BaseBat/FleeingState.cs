using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingState : AbsBaseState<BatStateMachine.BatStates>
{
    // Get movement controls
    KinematicSteer _MovementControls;

    // Constructor with call to base state class
    public FleeingState() : base(BatStateMachine.BatStates.Fleeing)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        BatStateMachine FSM = (BatStateMachine)OwnerFSM;
        _MovementControls = FSM.MovementControls;
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
        BatStateMachine FSM = (BatStateMachine)OwnerFSM;

        // Early return if movement controller is null
        if (_MovementControls == null)
            return;

        _MovementControls.isFleeing = true;

        // Check for stun
        FSM.DetectStun();

        // Check if destination is reached
        if (_MovementControls.IsAtDestination())
            FSM.Reset();
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: BatStates, state to transition to given current game conditions
	*/
    public override BatStateMachine.BatStates GetNextState()
    {
        return BatStateMachine.BatStates.Fleeing;
    }
}
