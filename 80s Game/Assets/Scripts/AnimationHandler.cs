using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    Animator anime; //animator for the given object
    Target self; //Target script for object
    private int stunHash = Animator.StringToHash("hit");
    private int timeHash = Animator.StringToHash("stunTime");
    private bool dying = false;
    private int deathTimer;
    private const int DTIME = 10;

    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponent<Animator>(); //set up animator
        self = GetComponent<Target>();
        deathTimer = DTIME;
        anime.SetInteger(timeHash, deathTimer);
    }

    // Update is called once per frame
    void Update()
    {
        if (dying != true && self.currentState == TargetStates.Death)
        {
            dying = true;
            anime.SetTrigger(stunHash);
        }
        if (dying == true && deathTimer > 0) //death animation should loop for ~10 frames
        {
            deathTimer--;
            anime.SetInteger(timeHash, deathTimer);
        }
        if (self.currentState != TargetStates.Death && dying == true) //target has been reset, needs to be updated
        {
            dying = false;
            deathTimer = DTIME;
            anime.SetInteger(timeHash, deathTimer);
            anime.ResetTrigger(stunHash);
        }
    }
}
