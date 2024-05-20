using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DeathState : AbsBaseState<BatStateMachine.BatStates>
{
    // Get movement controls
    KinematicSteer _MovementControls;

    // Constructor with call to base state class
    public DeathState() : base(BatStateMachine.BatStates.Death)
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

        // Disable movement and set bat fall movement
        _MovementControls.canMove = false;
        FSM.transform.position += new Vector3(0.0f, -1.0f, 0.0f) * 6.0f * Time.deltaTime;

        // Reset all target values once in this state if
        // bat has dropped
        if (FSM.transform.position.y <= FSM.deathHeight)
        {
            FSM.Reset();
        }
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: CreatureStates, state to transition to given current game conditions
	*/
    public override BatStateMachine.BatStates GetNextState()
    {
        return BatStateMachine.BatStates.Death;
    }
}
