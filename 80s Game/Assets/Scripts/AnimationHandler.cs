using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator anime; //animator for the given object

    private int _stunHash;
    private int _dropHash;
    private int _resetHash;
    private int _attackHash;

    // Start is called before the first frame update
    void Awake()
    {
        anime = GetComponent<Animator>(); //set up animator

        _stunHash = Animator.StringToHash("hit");
        _dropHash = Animator.StringToHash("drop");
        _resetHash = Animator.StringToHash("reset");
        _attackHash = Animator.StringToHash("attack");
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Sets the "hit" trigger for the animator, initiating the Stun animation
    /// </summary>
    public void PlayStunAnimation()
    {
        // Reset potentially active animations
        anime.ResetTrigger(_dropHash);
        anime.ResetTrigger(_resetHash);
        anime.ResetTrigger(_attackHash);

        // Set new animation
        anime.SetTrigger(_stunHash);
    }

    /// <summary>
    /// Sets the "drop" trigger for the animator, initiating the Drop animation
    /// </summary>
    public void PlayDropAnimation()
    {
        // Reset potentially active animations
        anime.ResetTrigger(_stunHash);
        anime.ResetTrigger(_resetHash);
        anime.ResetTrigger(_attackHash);

        // Set new animation
        anime.SetTrigger(_dropHash);
    }

    /// <summary>
    /// Sets the "attack" trigger for the animator, initiating the Attack animation
    /// </summary>
    public void PlayAttackAnimation()
    {
        // Reset potentially active animations
        anime.ResetTrigger(_stunHash);
        anime.ResetTrigger(_resetHash);
        anime.ResetTrigger(_dropHash);

        // Set new animation
        anime.SetTrigger(_attackHash);
        Debug.Log("playing attack animation");
    }

    /// <summary>
    /// Sets the "reset" trigger for the animator, resetting the animation state machine to the default Flight animation
    /// </summary>
    public void ResetAnimation()
    {
        // Reset potentially active animations
        anime.ResetTrigger(_stunHash);
        anime.ResetTrigger(_dropHash);
        anime.ResetTrigger(_attackHash);

        // Set new animation
        anime.SetTrigger(_resetHash);
        Debug.Log("Resetting to default animation");
    }
}
