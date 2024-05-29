using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerScoreUI : MonoBehaviour
{
    public List<TextMeshProUGUI> textScores;

    // Start is called before the first frame update
    void Start()
    {
        foreach (TextMeshProUGUI text in textScores)
        {
            text.text = "0";
        }
    }

    public void UpdateScores(int player)
    {
        textScores[player].text = GameManager.Instance.PointsManager.TotalPointsByPlayer[player].ToString();
    }
}
