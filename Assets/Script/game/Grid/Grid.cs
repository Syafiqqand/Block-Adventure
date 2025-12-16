using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squareGap = 0.1f;
    public GameObject gridSquare;  
    public Vector2 startPos = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0f;
    
    private Vector2 offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LIneIndicator _lineIndicator;
    public SquareTextureData squareTextureData;

    private Config.SquareColor currentActiveSquareColor_ = Config.SquareColor.notSet;
    private List<Config.SquareColor> colorsInTheGrid_ = new List<Config.SquareColor>();

    private void OnDisable()
    {
        GameEvent.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvent.UpdateSquaresColor -= UpdateSquareColors;
        GameEvent.CheckIfPlayerLost -= CheckIfPlayerLost;
    }

    private void OnEnable()
    {
        GameEvent.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvent.UpdateSquaresColor += UpdateSquareColors;
        GameEvent.CheckIfPlayerLost += CheckIfPlayerLost;
    }
    
    void Start()
    {
        _lineIndicator = GetComponent<LIneIndicator>();
        CreateGrid();
        currentActiveSquareColor_ = squareTextureData.ActiveSquareTexture[0].squareColor;
    }

    private void UpdateSquareColors(Config.SquareColor color)
    {
        currentActiveSquareColor_ = color;
    }

    private List<Config.SquareColor> GetAllColorsInTheGrid()
    {
        var colors = new List<Config.SquareColor>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.SquareOccupied)
            {
                var color = gridSquare.GetCurrentColor();
                if(colors.Contains(color) == false)
                {
                    colors.Add(color);
                }
            }
        }
        return colors;
    }
    

    private void CreateGrid()
    {
        SpawnGridSquare();
        SetGridSquarePosition();
    }

    private void SpawnGridSquare()
    {
        int square_index = 0;
        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                var squareObj = Instantiate(gridSquare) as GameObject;
                squareObj.transform.SetParent(this.transform);
                squareObj.transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                var gridSquareComp = squareObj.GetComponent<GridSquare>();
                gridSquareComp.SquareIndex = square_index;
                gridSquareComp.SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                _gridSquares.Add(squareObj);
                square_index++;
            }
        }
    }

    private void SetGridSquarePosition()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number >= columns)
            {
                square_gap_number.x = 0;
                // Move to next row
                column_number = 0;
                row_number++;
                row_moved = false;   
            }
            
            var pos_x_offset = offset.x * column_number + (square_gap_number.x * squareGap);
            var pos_y_offset = offset.y * row_number + (square_gap_number.y * squareGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squareGap;
            }

            if (row_number > 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squareGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x + pos_x_offset, startPos.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(square.GetComponent<RectTransform>().localPosition.x, square.GetComponent<RectTransform>().localPosition.y, 0.0f);
            column_number++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var selectedIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if (gridSquare.selected && !gridSquare.SquareOccupied)
            {
                selectedIndexes.Add(gridSquare.SquareIndex);
                gridSquare.selected = false;
            }
        }
        var currentSelectedShape = shapeStorage.GetCurenntSelectedShape();
        if (currentSelectedShape == null)
            return;

        if (currentSelectedShape.totalSquareNumber == selectedIndexes.Count)
        {
            foreach (var squareIndex in selectedIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(currentActiveSquareColor_);
            }


            var shapeLeft = 0;
            foreach (var shape in shapeStorage.shapelist)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }              
            }

            if (shapeLeft == 0)
            {
                GameEvent.RequestNewShape();
            }
            else
            {
                GameEvent.SetShapeInactive();
            }

            CheckIfCompletedLines();
        }
        else
        {
            GameEvent.MoveShapeBackToStartPosition();
        }
    }

    void CheckIfCompletedLines()
    {
        if (_lineIndicator == null)
        {
            Debug.LogError("CheckIfCompletedLines: LIneIndicator component is missing");
            return;
        }
        if (_gridSquares == null || _gridSquares.Count == 0)
        {
            Debug.LogWarning("CheckIfCompletedLines: grid squares not initialized");
            return;
        }
        if (_lineIndicator.lineData == null || _lineIndicator.squareData == null || _lineIndicator.columnIndexes == null)
        {
            Debug.LogError("CheckIfCompletedLines: line indicator data arrays are null");
            return;
        }
        List<int[]> lines = new List<int[]>();
        
        //col
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //row

        for (var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.lineData[row, index]);
            }
            lines.Add(data.ToArray());
        }

        //square
        for (var square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);

            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.squareData[square, index]);
            }
            lines.Add(data.ToArray());
        }
        //fungsi ini perlu dipanggil sebelum check completed lines
        colorsInTheGrid_ = GetAllColorsInTheGrid();

        var completedLines = CheckIfSquareAreCompleted(lines);
        if (completedLines >= 1)
        {
            GameEvent.ShowCongratulationWritings();
        }
        var totalScore = 10 * completedLines;
        var bonusScore = ShouldPlayColorBonusAnimation();
        GameEvent.AddScore(totalScore + bonusScore);
        GameEvent.CheckIfPlayerLost();
    }

    private int ShouldPlayColorBonusAnimation()

    {
        var colorsInTheGridAfterLineRemoved = GetAllColorsInTheGrid();
        Config.SquareColor colorToPlayBonusFor = Config.SquareColor.notSet;

        foreach (var squareColor in colorsInTheGrid_)
        {
            if (colorsInTheGridAfterLineRemoved.Contains(squareColor) == false)
            {
                colorToPlayBonusFor = squareColor;
            }
        }

        // Award bonus only when a color disappeared
        if (colorToPlayBonusFor == Config.SquareColor.notSet)
        {
            return 0;
        }

        // Show bonus for the disappeared color (including currentActiveSquareColor_)
        GameEvent.ShowBonusScreen(colorToPlayBonusFor);
        Debug.Log("Bonus: color disappeared " + colorToPlayBonusFor.ToString());
        return 50;
    }

    private int CheckIfSquareAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }
            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }
        foreach (var line in completedLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.clearOcupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShape = 0;

        for(var index = 0; index < shapeStorage.shapelist.Count; index++)
        {
            var isShapeActive = shapeStorage.shapelist[index].IsAnyOfShapeSquareActive();
            if(CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapelist[index]) && isShapeActive)
            {
                shapeStorage.shapelist[index]?.ActivateShape();
                validShape++;
            }
        }

        if(validShape == 0)
        {
            GameEvent.GameOver(false);
            Debug.Log("Game Over");
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        //all indexes of filled up square in grid
        List<int> originalShapefilledUpSquares = new List<int>();
        var squareIndex = 0;
        for (var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapefilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }
        if(currentShape.totalSquareNumber != originalShapefilledUpSquares.Count)
            Debug.LogError("mismatch filled up square number");
        
        var squareList = GetAllSquareCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;
            foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in number)
            {
                    var comp = _gridSquares[squareIndexToCheck].GetComponent<GridSquare>();
                if (comp.SquareOccupied == true)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if(shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }
    private List<int[]> GetAllSquareCombination(int Columns, int Rows)
    {
        var squareList = new List<int[]>();
        for (int startRow = 0; startRow <= 9 - Rows; startRow++)
        {
            for (int startCol = 0; startCol <= 9 - Columns; startCol++)
            {
                var rowData = new List<int>();
                for (int r = startRow; r < startRow + Rows; r++)
                {
                    for (int c = startCol; c < startCol + Columns; c++)
                    {
                        rowData.Add(_lineIndicator.lineData[r, c]);
                    }
                }
                squareList.Add(rowData.ToArray());
            }
        }
        return squareList;
    }
}