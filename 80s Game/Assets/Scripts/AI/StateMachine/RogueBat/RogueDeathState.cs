using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueDeathState : AbsBaseState<RogueBatStateMachine.RogueBatStates>
{
    // Get components
    SpriteRenderer _SpriteRenderer;

    // Constructor with call to base state class
    public RogueDeathState() : base(RogueBatStateMachine.RogueBatStates.Death)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        RogueBatStateMachine FSM = (RogueBatStateMachine)OwnerFSM;
        _SpriteRenderer = FSM.SpriteRenderer;
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
        // Get rogue bat FSM
        RogueBatStateMachine FSM = (RogueBatStateMachine)OwnerFSM;
        if (FSM == null)
            return;

        // Get needed components
        _SpriteRenderer = FSM.SpriteRenderer;

    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: BatStates, state to transition to given current game conditions
	*/
    public override RogueBatStateMachine.RogueBatStates GetNextState()
    {
        return RogueBatStateMachine.RogueBatStates.Death;
    }
}
