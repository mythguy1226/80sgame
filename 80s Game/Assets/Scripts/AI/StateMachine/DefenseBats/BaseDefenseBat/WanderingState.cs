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

        // Update flee timer
        FSM.fleeTimer -= Time.deltaTime;

    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: DefenseBatStates, state to transition to given current game conditions
	*/
    public override DefenseBatStateMachine.DefenseBatStates GetNextState()
    {
        
        return DefenseBatStateMachine.DefenseBatStates.Wandering;
    }
}
