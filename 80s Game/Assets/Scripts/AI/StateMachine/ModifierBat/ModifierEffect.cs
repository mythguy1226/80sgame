using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsModifierEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.detectHitSub += ListenForShot;
    }

    /// <summary>
    /// Abstract method each modifier class will implement
    /// to implement modifier effects
    /// </summary>
    public abstract void ActivateEffect();

    /// <summary>
    /// Checks if modifier has been shot
    /// </summary>
    public virtual void DetectHit(Vector3 pos)
    {
        bool isGameGoing = Time.timeScale > 0;
        if (!isGameGoing)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        // Null check
        if (!hit)
            return;

        // Check that hit has detected this particular object
        if (hit.collider.gameObject == gameObject)
        {
            ActivateEffect();
            InputManager.detectHitSub -= ListenForShot;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Listener event for detecting hits from input manager
    /// </summary>
    /// <param name="position">Position where player last fired</param>
    public void ListenForShot(Vector3 position)
    {
        DetectHit(position);
    }
}
