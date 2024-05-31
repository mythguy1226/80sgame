using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This script handles the auto scrolling for the dropdown in the Join Panel Color Settings
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectEnsureVisible : MonoBehaviour
{
    RectTransform scrollRectTransform;
    RectTransform contentPanel;
    RectTransform selectedRectTransform;
    GameObject lastSelected;

    public EventSystem eventSystem;

    void Start()
    {
        scrollRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        //just incase content panel gets created in start.
        if(contentPanel == null) contentPanel = GetComponent<ScrollRect>().content;

        //Set selected game object
        GameObject selected = eventSystem.currentSelectedGameObject;

        if (selected == null)
        {
            return;
        }
        if (selected.transform.parent != contentPanel.transform)
        {
            return;
        }
        if (selected == lastSelected)
        {
            return;
        }

        //Change the position of the content to fit with the selected object
        selectedRectTransform = selected.GetComponent<RectTransform>();
        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, - (selectedRectTransform.localPosition.y) - (selectedRectTransform.rect.height/2));

        lastSelected = selected;
    }
}
