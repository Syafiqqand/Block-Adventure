using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
   public Button OpenSettingsButton;
    public Button CloseSettingsButton;
    public Button ToggleMusicButton;

    private bool _musicMuted;

    private void Awake()
    {
        // Ensure references exist and set a safe initial state
        if (OpenSettingsButton != null)
        {
            OpenSettingsButton.gameObject.SetActive(true);
            OpenSettingsButton.interactable = true;
        }

        if (CloseSettingsButton != null)
        {
            CloseSettingsButton.gameObject.SetActive(false);
            CloseSettingsButton.interactable = false;
        }

        if (ToggleMusicButton != null)
        {
            ToggleMusicButton.gameObject.SetActive(false);
            ToggleMusicButton.interactable = false;
            ToggleMusicButton.onClick.AddListener(OnToggleMusicClicked);
        }
    }

    public void SettingsOpened()
    {
        if (OpenSettingsButton == null || CloseSettingsButton == null)
        {
            Debug.LogWarning("SettingsButton: Button references are missing.");
            return;
        }

        OpenSettingsButton.gameObject.SetActive(false);
        CloseSettingsButton.gameObject.SetActive(true);
        CloseSettingsButton.interactable = true;

        if (ToggleMusicButton != null)
        {
            ToggleMusicButton.gameObject.SetActive(true);
            ToggleMusicButton.interactable = true;
        }
    }

    public void SettingsClosed()
    {
        if (OpenSettingsButton == null || CloseSettingsButton == null)
        {
            Debug.LogWarning("SettingsButton: Button references are missing.");
            return;
        }

        OpenSettingsButton.gameObject.SetActive(true);
        CloseSettingsButton.gameObject.SetActive(false);
        OpenSettingsButton.interactable = true;

        if (ToggleMusicButton != null)
        {
            ToggleMusicButton.gameObject.SetActive(false);
            ToggleMusicButton.interactable = false;
        }
    }

    private void OnToggleMusicClicked()
    {
        _musicMuted = !_musicMuted;
        if (_musicMuted)
        {
            SoundManager.Instance?.PauseMusic();
        }
        else
        {
            SoundManager.Instance?.ResumeMusic();
        }
    }
}