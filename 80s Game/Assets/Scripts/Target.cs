using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isOnScreen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If on-screen move towards origin
        if(isOnScreen)
        {
            // Get the direction to move in
            Vector3 directionToOrigin = Vector3.Normalize(Vector3.zero - transform.position);

            // Update target position in direction
            transform.position += directionToOrigin * 2.0f * Time.deltaTime;
        }
    }
}
