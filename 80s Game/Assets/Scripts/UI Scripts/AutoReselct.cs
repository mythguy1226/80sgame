using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoReselct : MonoBehaviour
{
    private GameObject lastSelectedObject;

    // Update is called once per frame
    void Update()
    {
        //Always select the last selected game object (avoids clicking and being unable to navigate UI with keyboard)
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedObject); // no current selection, go back to last selected
        else
            lastSelectedObject = EventSystem.current.currentSelectedGameObject; // keep setting current selected object

    }
}
