using UnityEngine;

public class LatchPoint : MonoBehaviour
{
    public bool isAvailable = true;
    public Target latchedTarget;

    // Latch a target to this point
    public void LatchTarget(Target target)
    {
        latchedTarget = target;
        //target.transform.SetParent(transform);
        isAvailable = false;
    }


    // Release the latched target
    public void Unlatch()
    {
        //latchedTarget.transform.SetParent(null);
        isAvailable = true;

        // Reset target and FSM values as needed
        if(latchedTarget != null)
        {
            DefenseBatStateMachine FSM = (DefenseBatStateMachine) latchedTarget.FSM;
            FSM.targetLatch = null;
            FSM.SetPursueTimer();
            FSM.TransitionToDefault();
        }
        latchedTarget = null;
    }
}