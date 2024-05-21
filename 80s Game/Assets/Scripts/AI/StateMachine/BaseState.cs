using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: AbsBaseState
 * USAGE: Abstract class used as a template for 
 * creating state classes
 */
public abstract class AbsBaseState<EState> where EState : System.Enum
{
    /*
	USAGE: Constructor
	ARGUMENTS:
    -	EState key -> key to entry state
	*/
    public AbsBaseState(EState key)
    {
        StateKey = key;
    }

    // Public properties
    public AbsStateMachine<EState> OwnerFSM { get; set; }
    public EState StateKey { get; private set; }

    // Methods needed for state implementation
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();

    // Overrideable methods optional for implementation
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
}