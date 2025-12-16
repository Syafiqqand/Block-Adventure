using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquareTextureData", menuName = "ScriptableObjects/SquareTextureData", order = 1)]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite textureSprite;
        public  Config.SquareColor squareColor;
    }

    public int tresholdVal = 10;
    private const int startTreshold = 10;
    public List<TextureData> ActiveSquareTexture;

    public Config.SquareColor currentColor;
    private Config.SquareColor _nextColor;

    public int GetCurrentIndex = 0;


    public int GetCurrentColorIndex()
    {
        var currentIndex = 0;
        for (int index = 0; index < ActiveSquareTexture.Count; index++)
        {
            if(ActiveSquareTexture[index].squareColor == currentColor )
            {
                currentIndex = index;
                break;
            }
        }
        return currentIndex;
    }

    public void UpdateColors(int current_score)
    {
        currentColor = _nextColor;
        var currentColorIndex = GetCurrentColorIndex();
        if (currentColorIndex == ActiveSquareTexture.Count - 1)
            _nextColor = ActiveSquareTexture[0].squareColor;
        else
            _nextColor = ActiveSquareTexture[currentColorIndex + 1].squareColor;

            tresholdVal = startTreshold + current_score;
    }

    public void setInitialColor()
    {
        tresholdVal = startTreshold;
        currentColor = ActiveSquareTexture[0].squareColor;
        _nextColor = ActiveSquareTexture[1].squareColor;
    }

    private void Awake()
    {
        setInitialColor();
    }

    private void OnEnable()
    {
        setInitialColor();
    }
}
