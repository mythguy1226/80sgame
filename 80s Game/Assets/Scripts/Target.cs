using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetManager.TargetType type;
    // Get state machine
    public BatStateMachine FSM;

    // Call once upon start of game
    void Awake()
    {
        // Init component references
        FSM = GetComponent<BatStateMachine>();
    }
}
