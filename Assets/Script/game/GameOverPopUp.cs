using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject gameOverPopUp;
    public GameObject LosePopUp;
    public GameObject newBestScorePopUp;
    public TMPro.TextMeshProUGUI lastScoreText;

    void Start()
    {
        gameOverPopUp.SetActive(false);
        LosePopUp.SetActive(false);
        newBestScorePopUp.SetActive(false);
    }
    private void OnEnable()
    {
        GameEvent.GameOver += ShowGameOverPopUp;
    }  
    private void OnDisable()
    {
        GameEvent.GameOver -= ShowGameOverPopUp;
    }

    private void ShowGameOverPopUp(bool isNewBestScore)
    {
        StopAllCoroutines();
        StartCoroutine(ShowGameOverRoutine(isNewBestScore));
    }

    private IEnumerator ShowGameOverRoutine(bool isNewBestScore)
    {
        gameOverPopUp.SetActive(true);
        // Pause music while playing SFX
        SoundManager.Instance?.PauseMusic();
        int lastScore = PlayerPrefs.GetInt("lastScore", 0);
        if (lastScoreText != null)
        {
            lastScoreText.text = lastScore.ToString();
        }
        if (isNewBestScore)
        {
            newBestScorePopUp.SetActive(true);
            LosePopUp.SetActive(false);
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxClips[1]);
        }
        else
        {
            LosePopUp.SetActive(true);
            newBestScorePopUp.SetActive(false);
            SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxClips[2]);
        }
        // Wait 1 second then resume music
        yield return new WaitForSeconds(1f);
        SoundManager.Instance?.ResumeMusic();
    }
}
