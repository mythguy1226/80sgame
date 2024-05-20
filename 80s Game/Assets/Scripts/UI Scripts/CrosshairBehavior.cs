using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairBehavior : MonoBehaviour
{
    public static CrosshairBehavior Instance;

    //position the crosshair is mapped to
    private Vector3 position;

    void Awake()
    {
        //Make crosshair persistent across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Hide mouse cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCrosshair(Vector3 newPosition)
    {
        position = Camera.main.ScreenToWorldPoint(newPosition);
        position.z = 0;

        this.transform.position = position;
    }
}
