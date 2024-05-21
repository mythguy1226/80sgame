
using System.Collections.Generic;
public static class PlayerData
{
    public static List<PlayerController> activePlayers = new List<PlayerController>();

    public static void Reset()
    {
        activePlayers.Clear();
    }
}