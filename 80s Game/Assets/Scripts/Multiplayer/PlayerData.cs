
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Player configuration object that houses the data relevant to set up players for the game
public class PlayerConfig
{
    public Sprite crosshairSprite;
    public int crossHairIndex;
    public Color crossHairColor;
    public Vector2 sensitivity;
    public int playerIndex;
    public string initials;
    public string controlScheme;
    public InputDevice device;
    public PlayerConfig(int i, Color col, Vector2 sens)
    {
        crossHairColor = col;
        sensitivity = sens;
        playerIndex = i;
    }
}


// Static singleton that persists through game scenes.
// Houses the information currently active players
// Should be reset at the end of every game to avoid behavior problems
public static class PlayerData
{
    public static Color[] defaultColors =
    {
        new Color(1,1,1),
        new Color(1,0,0),
        new Color(0,1,0),
        new Color(1,1,0)
    };

    public static List<PlayerConfig> activePlayers = new List<PlayerConfig>();

    public static void Reset()
    {
        activePlayers.Clear();
    }
}