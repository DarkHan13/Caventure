using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{

    private int width;
    private int height;
    private Vector3 _originPosition;
    private float cellSize;
    public TGridObject[,] gridArray;

    public  Grid(int width, int height, float cellSize, Vector3 originPosition,
        Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        _originPosition = originPosition;
        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Vector3 pos = GetWorldPosition(x, y);
                gridArray[x, y] = createGridObject(this, (int)pos.x, (int)pos.y);
            }
        }
        
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + _originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - _originPosition).y / cellSize);
    }
    
    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        return default;
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}
