using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapelist;

    void Start()
    {
     foreach (var shape in shapelist)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }   
    }
    void OnEnable()
    {
        GameEvent.RequestNewShape += RequestNewShape;
    }
    void OnDisable()
    {
        GameEvent.RequestNewShape -= RequestNewShape;
    }
    private void RequestNewShape()
    {
        if (shapeData == null || shapeData.Count == 0 || shapelist == null)
        {
            Debug.LogWarning("No shape data or shapelist available to create new shapes.");
            return;
        }

        foreach (var shape in shapelist)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }
    public Shape GetCurenntSelectedShape()
    {
        foreach (var shape in shapelist)
        {
            if (shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
                return shape;
        }
        Debug.LogError("No Shape Selected");
        return null;
    }
}
