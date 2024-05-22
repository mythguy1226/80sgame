using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    public PlayerController playerPrefab;

    // Public properties
    public InputManager InputManager { get; private set;  }
    public TargetManager TargetManager { get; private set; }
    public PointsManager PointsManager { get; private set; }
    public HitsManager HitsManager { get; private set; }

    public UIManager UIManager { get; private set; }

    private void Awake()
    {
        // Check if the static reference matches the script instance
        if(Instance != null && Instance != this)
        {
            // If not, then the script is a duplicate and can delete itself
            Destroy(this);
        }

        else
        {
            Instance = this;

            // Initialize managers
            InputManager = GetComponent<InputManager>();
            TargetManager = GetComponent<TargetManager>();
            PointsManager = GetComponent<PointsManager>();
            HitsManager = GetComponent<HitsManager>();
            UIManager = GetComponent<UIManager>();

            if (PlayerData.activePlayers.Count == 0) {
                PlayerConfig defaultConfig = new PlayerConfig(0, PlayerData.defaultColors[0], Vector2.one);
                PlayerData.activePlayers.Add(defaultConfig);
                PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                pc.SetConfig(defaultConfig);
            } else
            {
                for(int i = 0; i < PlayerData.activePlayers.Count; i++)
                {
                    PlayerController pc = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                    pc.SetConfig(PlayerData.activePlayers[i]);
                }
            }
        }
    }
}