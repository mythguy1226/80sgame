using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: AbsStateMachine
 * USAGE: Abstract class used as a template for 
 * creating state machines
 */
public abstract class AbsStateMachine<EState> : MonoBehaviour where EState : System.Enum
{
    // Public fields
    public bool bIsActive = true;

    // Protected fields
    protected Dictionary<EState, AbsBaseState<EState>> states = new Dictionary<EState, AbsBaseState<EState>>();
    protected AbsBaseState<EState> currentState;
    protected bool bIsTransitioningState = false;

    // Start is called at beginning of play
    void Start()
    {
        // Iterate through each state to pass game object as owner reference
        foreach (KeyValuePair<EState, AbsBaseState<EState>> state in states)
        {
            state.Value.OwnerFSM = this;
        }

        // Enter the first state
        currentState.EnterState();
    }

    // Update is called once every frame
    void Update()
    {
        // Early return if activity is disabled
        if (!bIsActive)
            return;

        // Get the next key from the current state
        // if a transition is ready
        EState nextStateKey = currentState.GetNextState();

        // Run current state's update when not transitioning
        if (!bIsTransitioningState && nextStateKey.Equals(currentState.StateKey))
            currentState.UpdateState();
        else // Transition case
            TransitionToState(nextStateKey);
    }

    /*
	USAGE: Transitions current state of state machine into a new state
	ARGUMENTS:
    -	EState stateKey -> state to transition to
	OUTPUT: ---
	*/
    public void TransitionToState(EState stateKey)
    {
        // Exit the current state,
        // set the new state
        // enter that new state
        bIsTransitioningState = true;
        currentState.ExitState();
        currentState = states[stateKey];
        currentState.EnterState();
        bIsTransitioningState = false;
    }

    /*
	USAGE: Calls upon state trigger handler for enter
	ARGUMENTS:
    -	Collider other -> collider that owning game object triggered
	OUTPUT: ---
	*/
    void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    /*
	USAGE: Calls upon state trigger handler for stay
	ARGUMENTS:
    -	Collider other -> collider that owning game object triggered
	OUTPUT: ---
	*/
    void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
    }

    /*
	USAGE: Calls upon state trigger handler for exit
	ARGUMENTS:
    -	Collider other -> collider that owning game object triggered
	OUTPUT: ---
	*/
    void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
}
