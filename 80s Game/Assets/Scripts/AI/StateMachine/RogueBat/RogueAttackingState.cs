using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueAttackingState : AbsBaseState<RogueBatStateMachine.RogueBatStates>
{
    // Get components
    SpriteRenderer _SpriteRenderer;

    // Constructor with call to base state class
    public RogueAttackingState() : base(RogueBatStateMachine.RogueBatStates.Attacking)
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

        // Update death timer
        FSM.deathTimer -= Time.deltaTime;

        // Set bat to face direction of its target
        Target curTarget = FSM.currentTarget;
        Vector3 facingDir = curTarget.transform.position - FSM.transform.position;
        facingDir.Normalize();
        if(facingDir.x > 0)
        {
            _SpriteRenderer.flipX = true;

            // Keep shot location on right side
            FSM.shootLocation.position = FSM.rightShotLoc.position;
        }
        else
        {
            _SpriteRenderer.flipX = false;

            // Keep shot location on left side
            FSM.shootLocation.position = FSM.leftShotLoc.position;
        }

        // Manage shot timer and shooting
        if(FSM.CanShoot) // Attack logic
        {
            FSM.LaunchProjectile();
            FSM.CanShoot = false;
        }
        else // Cooldown logic
        {
            FSM.ShotTimer -= Time.deltaTime;
            if(FSM.ShotTimer <= 0.0f)
            {
                FSM.ShotTimer = FSM.shotCooldown;
                FSM.CanShoot = true;
            }
        }
    }

    /*
	USAGE: Handler for state transitions
	ARGUMENTS: ---
	OUTPUT: BatStates, state to transition to given current game conditions
	*/
    public override RogueBatStateMachine.RogueBatStates GetNextState()
    {
        RogueBatStateMachine FSM = (RogueBatStateMachine)OwnerFSM;

        // When timer is up, kill the bat
        if(FSM != null)
        {
            // Check death timer
            if (FSM.deathTimer <= 0.0f)
            {
                return RogueBatStateMachine.RogueBatStates.Death;
            }

            // Check for closest targets
            FSM.currentTarget = FSM.GetClosestTarget();
            if(FSM.currentTarget == null)
                return RogueBatStateMachine.RogueBatStates.Observing;
        }
        
        return RogueBatStateMachine.RogueBatStates.Attacking;
    }
}
