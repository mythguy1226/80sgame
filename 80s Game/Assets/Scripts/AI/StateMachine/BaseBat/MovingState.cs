using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : AbsBaseState<BatStateMachine.BatStates>
{
    // Get components
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;

    // Constructor with call to base state class
    public MovingState() : base(BatStateMachine.BatStates.Moving)
    { }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        //BatStateMachine FSM = (BatStateMachine)OwnerFSM;
        //_MovementControls = FSM.MovementControls;
        //_SpriteRenderer = FSM.SpriteRenderer;
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

        // Check for stun
        FSM.DetectStun();
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: BatStates, state to transition to given current game conditions
	*/
    public override BatStateMachine.BatStates GetNextState()
    {
        BatStateMachine FSM = (BatStateMachine)OwnerFSM;

        // When timer is up, set target to flee
        if(FSM != null)
        {
            if (FSM.fleeTimer <= 0.0f)
            {
                // Get max height and width values from screen
                float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
                float maxWidth = maxHeight * (Screen.width / Screen.height);

                // Find random x position
                FSM.fleeLocation.x = UnityEngine.Random.Range((-maxWidth * 2) + _SpriteRenderer.size.x, (maxWidth * 2) - _SpriteRenderer.size.x);
                _MovementControls.SetTargetPosition(FSM.fleeLocation);

                return BatStateMachine.BatStates.Fleeing;
            }
        }
        
        return BatStateMachine.BatStates.Moving;
    }
}
