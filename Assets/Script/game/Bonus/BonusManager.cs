using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    private void Start()
    {
        GameEvent.ShowBonusScreen += ShowBonusScreen;
    }
    private void OnDisable()
    {
        GameEvent.ShowBonusScreen -= ShowBonusScreen;
    }

    private void ShowBonusScreen(Config.SquareColor color)
    {
        GameObject obj = null;
        foreach (var bonus in bonusList)
        {
            var bonusComponent = bonus.GetComponent<Bonus>();
            if (bonusComponent.color == color)
            {
                obj = bonus;
                bonus.SetActive(true);
            }
        }
        StartCoroutine(DeactivateBonus(obj));
    }   

    private IEnumerator DeactivateBonus(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.SetActive(false);
    }   
}
