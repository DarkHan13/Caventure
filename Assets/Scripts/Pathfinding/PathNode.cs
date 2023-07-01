using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

    private Grid<PathNode> grid;
    public int x;
    public int y;
    public bool isFloor;
    public int height = 0;
    private Vector3 position;

    public int gCost, hCost, fCost;

    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        position = new Vector3(x, y);
    }

    public PathNode(Grid<PathNode> grid, Vector3Int pos)
    {
        this.grid = grid;
        x = pos.x;
        y = pos.y;
        position = pos;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return $"{x}, {y}";
    }
}
