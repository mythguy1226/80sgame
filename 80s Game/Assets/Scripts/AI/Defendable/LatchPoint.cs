using UnityEngine;

public class LatchPoint : MonoBehaviour
{
    public bool isAvailable;
    public Target latchedTarget;

    // Latch a target to this point
    public void LatchTarget(Target target)
    {
        latchedTarget = target;
        target.transform.SetParent(transform);
        isAvailable = false;
    }


    // Release the latched target
    public void Unlatch()
    {
        latchedTarget.transform.SetParent(null);
        latchedTarget = null;
        isAvailable = true;
    }
}