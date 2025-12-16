using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int Score;
}

public class Score : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public TextMeshProUGUI scoreTextMesh;
    private bool newBestScore = false;
    private BestScoreData bestScores_ = new BestScoreData();
    private int currentScore_;

    private string bestScoreKey_ = "bestScoreData";

    private void Awake()
    {
        if(BinaryDataStream.Exists(bestScoreKey_))
        {
            StartCoroutine(ReadDatafile());
        }
    }

    private IEnumerator ReadDatafile()
    {        
        bestScores_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        GameEvent.UpdateBestScoreBar?.Invoke(currentScore_, bestScores_.Score);
    }


    void Start()
    {
        currentScore_ = 0;
        newBestScore = false;
        squareTextureData.setInitialColor();
        UpdateScoreText();
    }

    public void OnEnable()
    {
        GameEvent.AddScore += AddScore;
        GameEvent.GameOver += SaveBestScore;
        }

    public void OnDisable()
    {
        GameEvent.AddScore -= AddScore;
        GameEvent.GameOver -= SaveBestScore;
    }

    public void SaveBestScore(bool isNewBestScore)
    {
        PlayerPrefs.SetInt("lastScore", currentScore_);
        PlayerPrefs.Save();
        BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey_);
    }

    private void AddScore(int scores)
    {
        currentScore_ += scores;
        if(currentScore_ > bestScores_.Score)
        {
            newBestScore = true;
            bestScores_.Score = currentScore_; 
            SaveBestScore(true);       
        }

        UpdateSquareColors();
        GameEvent.UpdateBestScoreBar?.Invoke(currentScore_, bestScores_.Score);
        UpdateScoreText();
    }

    private void UpdateSquareColors()
    {
        if(GameEvent.UpdateSquaresColor != null && currentScore_ >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScore_);
            GameEvent.UpdateSquaresColor?.Invoke(squareTextureData.currentColor);
        }
    }

    private void UpdateScoreText()
    {
        scoreTextMesh.text = currentScore_.ToString();
    }
}
