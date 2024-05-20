using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{

    // Get state machine
    public BatStateMachine FSM;

    // Call once upon start of game
    void Awake()
    {
        // Init component references
        FSM = GetComponent<BatStateMachine>();
    }
}
