using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineWrapper : MonoBehaviour
{
    public abstract bool IsActive();
    public abstract bool IsDefault();
    public abstract void SetActive(bool newActivity);
    public abstract void TransitionToDefault();
    public abstract void TransitionToTerminal();
    public abstract bool InUnscorableState();

    public virtual void ResolveEvent(){}

    public abstract string GetCurrentState();
}
