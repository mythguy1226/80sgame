using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : AbsBaseState<DefenseBatStateMachine.DefenseBatStates>
{
    // Get components
    KinematicSteer _MovementControls;
    SpriteRenderer _SpriteRenderer;
    AnimationHandler _AnimControls;

    // Constructor with call to base state class
    public AttackingState(AnimationHandler animationHandler) : base(DefenseBatStateMachine.DefenseBatStates.Attacking)
    {
        this._AnimControls = animationHandler;
    }

    /*
	USAGE: Handler for state enter
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void EnterState()
    {
        OwnerFSM.GetComponent<CircleCollider2D>().isTrigger = false;
        OwnerFSM.GetComponent<PolygonCollider2D>().isTrigger = false;
        OwnerFSM.gameObject.GetComponent<Target>().bIsStunned = false;
        _AnimControls.SetLatched(true);
    }

    /*
	USAGE: Handler for state exit
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public override void ExitState()
    {
        // Any clean up needed from this state will go here
        _AnimControls.SetLatched(false);
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
        _MovementControls.canMove = false;

        // Manage attack timer and attacking
        if(FSM.CanAttack) // Attack logic
        {
            _AnimControls.PlayAttackAnimation();
            FSM.CanAttack = false;
        }
        else // Cooldown logic
        {
            FSM.AttackTimer -= Time.deltaTime;
            if(FSM.AttackTimer <= 0.0f)
            {
                FSM.AttackTimer = FSM.attackCooldown;
                FSM.CanAttack = true;
            }
        }
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
            if(Vector3.Distance(FSM.targetLatch.transform.position, FSM.transform.position) > 1.0f)
            {
                return DefenseBatStateMachine.DefenseBatStates.Pursuing;
            }
        }
        return DefenseBatStateMachine.DefenseBatStates.Attacking;
    }
}
