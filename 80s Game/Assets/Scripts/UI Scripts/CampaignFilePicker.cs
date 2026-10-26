
using UnityEngine;
using SteamIntegration;
using System.Collections.Generic;

public class CampaignFilePicker : MonoBehaviour
{
    GameFileSaver gameFileSaver;

    [SerializeField]
    List<GameFileCard> gameFiles;


    private void OnEnable()
    {
        CampaignSave.Reset();
        SteamInterface steam = GameManager.Instance.GetComponent<SteamInterface>();
        // To do: Implement Steam Saver
        if (false)
        {
            if (steam.IsConnected)
            {
                gameFileSaver = new SteamFileSaver();
                
            } else
            {
                gameFileSaver = new IOFileSaver();
            }
        } else
        {
            gameFileSaver = new IOFileSaver();
        }
        CampaignSave.FileSaver = gameFileSaver;
        DisplayFiles();

    }

    private void DisplayFiles()
    {
        for (int i = 0; i < gameFiles.Count; i++) {
            GameFileData data = gameFileSaver.LoadFile(i);
            GameFileCard card = gameFiles[i];
            card.SetUp(i, data);
        }
    }
}