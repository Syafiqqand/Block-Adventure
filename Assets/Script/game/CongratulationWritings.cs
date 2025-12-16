using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationWritings : MonoBehaviour
{
    public List<GameObject> Writings;

    void Start()
    {
        GameEvent.ShowCongratulationWritings += ShowCongratulationWritings;
    }
    private void OnDisable()
    {
        GameEvent.ShowCongratulationWritings -= ShowCongratulationWritings;
    }

    private void ShowCongratulationWritings()
    {
        var index = UnityEngine.Random.Range(0, Writings.Count);
        Writings[index].SetActive(true);
    }
}
