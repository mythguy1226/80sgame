using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetManager.TargetType type;
    // Get state machine
    public StateMachineWrapper FSM;

    // Call once upon start of game
    void Awake()
    {
        // Init component references
        FSM = GetComponent<StateMachineWrapper>();
    }
}
