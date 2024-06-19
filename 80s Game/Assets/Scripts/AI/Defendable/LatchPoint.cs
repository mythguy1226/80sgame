using System.Threading;
using UnityEngine;

public class LatchPoint : MonoBehaviour
{
    public bool isAvailable;
    public Target latchedTarget;

    public void LatchTarget(Target target)
    {
        latchedTarget = target;
        target.transform.SetParent(transform);
        isAvailable = false;
    }
}