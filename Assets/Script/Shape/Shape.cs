using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImagePrefab;
    public Vector3 shapeSelectedoScale;
    public Vector2 offset = new Vector2(0f, 700f);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    public int totalSquareNumber {get; set;}

    private List<GameObject> _currentShapeSquares = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool shapeDragged = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        shapeDragged = true;
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }

    void OnEnable()
    {
        GameEvent.MoveShapeBackToStartPosition += MoveShapeBackToStartPosition;
        GameEvent.SetShapeInactive += SetShapeInactive;
    }

    void OnDisable()
    {
        GameEvent.MoveShapeBackToStartPosition -= MoveShapeBackToStartPosition;
        GameEvent.SetShapeInactive -= SetShapeInactive;
    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShapeSquares)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    private void SetShapeInactive()
    {
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShapeSquares)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShapeSquares)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
            _shapeActive = false;
        }
    }

    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShapeSquares)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
            _shapeActive = true;
        }
    }

    void Start()
     {
        
     }

    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        totalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShapeSquares.Count < totalSquareNumber)
        {
            _currentShapeSquares.Add(Instantiate(squareShapeImagePrefab, transform) as GameObject);
        }
        foreach (var square in _currentShapeSquares)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareReact = squareShapeImagePrefab.GetComponent<RectTransform>();
        var moveDistance = new Vector2(
            squareReact.rect.width * squareReact.transform.localScale.x,
            squareReact.rect.height * squareReact.transform.localScale.y
        );

        int currentIndexInList = 0;
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    float posX = GetXPositionForShapeSquare(shapeData, column, moveDistance);
                    float posY = GetYPositionForShapeSquare(shapeData, row, moveDistance);

                    _currentShapeSquares[currentIndexInList].gameObject.SetActive(true);
                    _currentShapeSquares[currentIndexInList].GetComponent<RectTransform>().localPosition =
                    new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance), 
                    GetYPositionForShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;
                }
            }
        }
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (shapeData.columns > 1)
        {
            float startXPos;
            if (shapeData.columns % 2 != 0)
                startXPos = (shapeData.columns / 2) * moveDistance.x * -1;
            else
                startXPos = ((shapeData.columns / 2) - 1) * moveDistance.x * -1 - moveDistance.x / 2;
            shiftOnX = startXPos + column * moveDistance.x;

        }
        return shiftOnX;
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (shapeData.rows > 1)
        {
            float startYPos;
            if (shapeData.rows % 2 != 0)
                startYPos = (shapeData.rows / 2) * moveDistance.y;
            else
                startYPos = ((shapeData.rows / 2) - 1) * moveDistance.y + moveDistance.y / 2;
            shiftOnY = startYPos - row * moveDistance.y;
        }
        return shiftOnY;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }
        return number;
    }
   public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedoScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera useCam = null;
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera || _canvas.renderMode == RenderMode.WorldSpace)
        {
            useCam = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
        }

        Vector2 localPoint;
        RectTransform canvasRect = _canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            useCam,
            out localPoint);

        _transform.anchoredPosition = localPoint + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvent.CheckIfShapeCanBePlaced?.Invoke();
        SoundManager.Instance.PlaySFX(SoundManager.Instance.sfxClips[0]);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    } 
    private void MoveShapeBackToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}
