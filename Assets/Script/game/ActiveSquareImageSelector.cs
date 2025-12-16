using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool updateImageOnreachTreshold = false;

    private void OnEnable()
    {
        UpdateSquareBasedOnCurrentPoint();

        if (updateImageOnreachTreshold)
        {
            GameEvent.UpdateSquaresColor += UpdateSquaresColor;
        }
    }
    private void OnDisable()
    {
        if (updateImageOnreachTreshold)
        {
            GameEvent.UpdateSquaresColor -= UpdateSquaresColor;
        }
    }

    private void UpdateSquareBasedOnCurrentPoint()
    {
        foreach (var squareTexture in squareTextureData.ActiveSquareTexture)
        {
            if (squareTextureData.currentColor == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.textureSprite;
            }
        }
    }

    private void UpdateSquaresColor(Config.SquareColor color)
    {
        foreach (var squareTexture in squareTextureData.ActiveSquareTexture)
        {
            if (color == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.textureSprite;
            }
        }
    }
}