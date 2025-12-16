using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class RequestNewShapeButton : MonoBehaviour
{
    public int nummberOfRequest = 3;
    public Button ActivateRequestButton;
    public TextMeshProUGUI NumberRequestText_;

    private int _currentRequest;
    private Button _button;
    private bool _isLocked;

    void Start()
    {
        NumberRequestText_.text = _currentRequest.ToString();
        _currentRequest = nummberOfRequest;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonDown);
        unlock();
    }

    private void OnButtonDown()
    {
        if (_isLocked == false)
        {
            _currentRequest--;
            GameEvent.RequestNewShape?.Invoke();
            GameEvent.CheckIfPlayerLost?.Invoke();
            
            if (_currentRequest <= 0)
            {
                Lock();
            }

            NumberRequestText_.text = _currentRequest.ToString();
        }
    }
    private void Lock()
    {
        _isLocked = true;
        _button.interactable = false;
        NumberRequestText_.text = _currentRequest.ToString();
    }

    private void unlock()
    {
        _isLocked = false;
        _button.interactable = true;
        NumberRequestText_.text = _currentRequest.ToString();
    }
}
