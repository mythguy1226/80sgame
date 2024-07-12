using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public List<Sprite> skipPromptIcons;
    public Image skipPrompt;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        switch (PlayerData.activePlayers[0].controlScheme)
        {
            case "PS4":
                skipPrompt.sprite = skipPromptIcons[0];
                break;
            case "xbox":
                skipPrompt.sprite = skipPromptIcons[1];
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SkipCutscene()
    {
        this.gameObject.GetComponent<PlayableDirector>().time = 4.2f;
    }
}
