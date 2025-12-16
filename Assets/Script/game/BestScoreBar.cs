using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI bestScoreText;

    private void OnEnable()
    {
        GameEvent.UpdateBestScoreBar += UpdateBestScoreBar;
    }
    private void OnDisable()
    {
        GameEvent.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int currentScore, int bestScore)
    {
        float currentPercentage = (float)currentScore / (float) bestScore;
        fillImage.fillAmount = currentPercentage;
        bestScoreText.text = bestScore.ToString();
    }
}