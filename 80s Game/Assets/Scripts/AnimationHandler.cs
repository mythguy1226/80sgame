using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator _animator; //_animator for the given object

    private int _stunHash;
    private int _dropHash;
    private int _resetHash;
    private int _attackHash;
    private int _chargeHash;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>(); //set up _animator

        _stunHash = Animator.StringToHash("hit");
        _dropHash = Animator.StringToHash("drop");
        _resetHash = Animator.StringToHash("reset");
        _attackHash = Animator.StringToHash("attack");
        _chargeHash = Animator.StringToHash("charge");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetLatched(bool isLatched)
    {
        _animator.SetBool("isLatched", isLatched);
    }

    /// <summary>
    /// Sets the "hit" trigger for the _animator, initiating the Stun animation
    /// </summary>
    public void PlayStunAnimation()
    {
        // Reset potentially active animations
        _animator.ResetTrigger(_dropHash);
        _animator.ResetTrigger(_resetHash);
        _animator.ResetTrigger(_attackHash);
        _animator.ResetTrigger(_chargeHash);

        // Set new animation
        _animator.SetTrigger(_stunHash);
    }

    /// <summary>
    /// Sets the "drop" trigger for the _animator, initiating the Drop animation
    /// </summary>
    public void PlayDropAnimation()
    {
        // Reset potentially active animations
        _animator.ResetTrigger(_stunHash);
        _animator.ResetTrigger(_resetHash);
        _animator.ResetTrigger(_attackHash);
        _animator.ResetTrigger(_chargeHash);

        // Set new animation
        _animator.SetTrigger(_dropHash);
    }

    /// <summary>
    /// Sets the "attack" trigger for the _animator, initiating the Attack animation
    /// NOTE: This should be used for the animation(s) that coincide with bats <i>damaging structures</i>:
    /// in the case of the Divebomb bat, this would be the divebomb (glide) that results in its explosion.
    /// </summary>
    public void PlayAttackAnimation()
    {
        // Reset potentially active animations
        _animator.ResetTrigger(_stunHash);
        _animator.ResetTrigger(_resetHash);
        _animator.ResetTrigger(_dropHash);
        _animator.ResetTrigger(_chargeHash);

        // Set new animation
        _animator.SetTrigger(_attackHash);
    }

    /// <summary>
    /// Sets the "charge" trigger for the _animator, initiating the Charge animation
    /// NOTE (07/11/2024): this should only apply to Divebomb bats, though as a precaution all _animators have this trigger in place.
    /// </summary>
    public void PlayChargeAnimation()
    {
        // Reset potentially active animations
        _animator.ResetTrigger(_stunHash);
        _animator.ResetTrigger(_dropHash);
        _animator.ResetTrigger(_attackHash);
        _animator.ResetTrigger(_resetHash);

        // Set new animation
        _animator.SetTrigger(_chargeHash);
    }

    /// <summary>
    /// Sets the "reset" trigger for the _animator, resetting the animation state machine to the default Flight animation
    /// </summary>
    public void ResetAnimation()
    {
        // Reset potentially active animations
        _animator.ResetTrigger(_stunHash);
        _animator.ResetTrigger(_dropHash);
        _animator.ResetTrigger(_attackHash);
        _animator.ResetTrigger(_chargeHash);

        // Set new animation
        _animator.SetTrigger(_resetHash);
    }
}
