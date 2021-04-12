using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI BestScoreText;
    private int playerScore = 0;
    private int bestPlayerScore;

    void Start()
    {
        bestPlayerScore = PlayerPrefs.GetInt("BestPlayerScore");
        UpdateView();
    }
    public void ScoreUpdater(int matchCount)
    {
        matchCount = GameController.Instance.matchCount;
        playerScore += matchCount;
        UpdateView();
    }
    public void UpdateView()
    {
        string msg = "Score: " + playerScore;
        ScoreText.text = msg;
        BestScoreText.text = ("Best: " + bestPlayerScore);

        if (playerScore > bestPlayerScore)
        {
            bestPlayerScore = playerScore;
            PlayerPrefs.SetInt("BestPlayerScore", bestPlayerScore);
        }
    }
}