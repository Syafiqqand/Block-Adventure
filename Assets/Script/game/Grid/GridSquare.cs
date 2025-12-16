using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;

    private Config.SquareColor currentSquareColor_= Config.SquareColor.notSet;

    public Config.SquareColor GetCurrentColor()
    {
        return currentSquareColor_;
    }

    public bool selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    void Start()
    {
        selected = false;
        SquareOccupied = false;
    }
    //temporary function to test grid square usability
    public bool CanWeUseThisGridSquare()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard(Config.SquareColor squareColor)
    {
        currentSquareColor_ = squareColor;
        ActivateSquare();
    }
        

    public void PlaceShapeOnBoard()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        SquareOccupied = true;
    }

    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        selected = true;
        SquareOccupied = true;
    }

    public void Deactivate()
    {
        currentSquareColor_ = Config.SquareColor.notSet;
        activeImage.gameObject.SetActive(false);
    }
    
    public void clearOcupied()
    {
        currentSquareColor_ = Config.SquareColor.notSet;
        SquareOccupied = false;
        selected = false;
    }

    public void SetImage(bool setFirstImage)
    {
        if (normalImage == null || normalImages == null || normalImages.Count < 2)
        {
            return;
        }
        normalImage.sprite = setFirstImage ? normalImages[0] : normalImages[1];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if( collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        selected = true;
        if (SquareOccupied == false)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if( collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            selected = false;
            hoverImage.gameObject.SetActive(false);
        }
        else if( collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();
        }
    }
}
