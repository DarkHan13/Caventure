using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding instance;
    
    private Grid<PathNode> _grid;

    public Grid<PathNode> grid
    {
        get => _grid;
    }

    private HashSet<Vector3Int> _emptyTilePositions;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(HashSet<Vector3Int> emptyTile, 
        HashSet<Vector3Int> floorList = null,
        Dictionary<Vector3Int, int> platformList = null)
    {
        instance = this;
        int xMax = int.MinValue, xMin = int.MaxValue,
            yMax = int.MinValue, yMin = int.MaxValue;
        _emptyTilePositions = emptyTile;
        foreach (var v3 in emptyTile)
        {
            if (v3.x > xMax) xMax = v3.x;
            if (v3.x < xMin) xMin = v3.x;
            if (v3.y > yMax) yMax = v3.y;
            if (v3.y < yMin) yMin = v3.y;
        }

        int width = Mathf.Abs(xMax - xMin);
        int height = Mathf.Abs(yMax - yMin);

        _grid = new Grid<PathNode>(width, height, 1f, new Vector3(xMin, yMin, 0), 
            (g, x, y) => new PathNode(g, x, y));
        
        // set height for each ceil
        for (int x = 0; x < _grid.gridArray.GetLength(0); x++)
        {
            int cellHeight = 0;
            for (int y = 0; y < _grid.gridArray.GetLength(1); y++)
            {
                if (!IsCellEmpty(_grid.gridArray[x, y])) cellHeight = 0;
                else
                {
                    cellHeight++;
                    _grid.gridArray[x, y].height = cellHeight;
                }
            }
        }

        if (floorList != null)
        {
            foreach (var tilePos in floorList)
            {
                var node = _grid.GetValue(tilePos);
                if (node != null) node.isFloor = true;
                else
                {
                    node = new PathNode(_grid, tilePos);
                    node.isFloor = true;
                    _grid.SetValue(tilePos, node);
                }
            }
        }
        if (platformList != null)
        {
            foreach (var (tilePos, value) in platformList)
            {
                var correctPos = tilePos + Vector3Int.up;
                var node = _grid.GetValue(correctPos);
                if (node != null) node.isFloor = true;
                else
                {
                    node = new PathNode(_grid, correctPos);
                    node.isFloor = true;
                    _grid.SetValue(correctPos, node);
                }    
            }
        }
    }
    
    
    public List<PathNode> FindPath(Vector3 start, Vector3 end)
    {
        PathNode startNode = _grid.GetValue(start);
        PathNode endNode = _grid.GetValue(end);

        if (endNode == null || startNode == null) return null;  

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();
        
        foreach (var tilePosition in _emptyTilePositions)
        {
            PathNode node = _grid.GetValue(tilePosition);
            if (node == null) continue;
            node.gCost = int.MaxValue;
            node.CalculateFCost();
            node.cameFromNode = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode) || neighbourNode == null) continue;

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        
        // Out of nodes on the openList
        Debug.Log("Doesn't work");
        return null;
    }

    
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        Vector3Int left = new Vector3Int(currentNode.x - 1, currentNode.y);
        Vector3Int right = new Vector3Int(currentNode.x + 1, currentNode.y);
        Vector3Int up = new Vector3Int(currentNode.x, currentNode.y + 1);
        Vector3Int down = new Vector3Int(currentNode.x, currentNode.y - 1);
        Vector3Int leftUp = new Vector3Int(currentNode.x - 1, currentNode.y + 1);
        Vector3Int rightUp = new Vector3Int(currentNode.x + 1, currentNode.y + 1);
        Vector3Int leftDown = new Vector3Int(currentNode.x - 1, currentNode.y - 1);
        Vector3Int rightDown = new Vector3Int(currentNode.x + 1, currentNode.y - 1);

        // Неправильно работает, но работает
        if (_emptyTilePositions.Contains(left))
        {
            neighbourList.Add(_grid.GetValue(left));
            if (_emptyTilePositions.Contains(leftUp)) neighbourList.Add(_grid.GetValue(leftUp));
            if (_emptyTilePositions.Contains(leftDown)) neighbourList.Add(_grid.GetValue(leftDown));
        }
        if (_emptyTilePositions.Contains(right))
        {
            neighbourList.Add(_grid.GetValue(right));
            if (_emptyTilePositions.Contains(rightUp)) neighbourList.Add(_grid.GetValue(rightUp));
            if (_emptyTilePositions.Contains(rightDown)) neighbourList.Add(_grid.GetValue(rightDown));
        }
        
        if (_emptyTilePositions.Contains(up)) neighbourList.Add(_grid.GetValue(up));
        if (_emptyTilePositions.Contains(down)) neighbourList.Add(_grid.GetValue(down));

        return neighbourList;

    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) 
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        if (a == null || b == null) return int.MaxValue;
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int CalculateDistanceCostPhysic(PathNode a, PathNode b, float jumpHeight)
    {
        if (a == null || b == null) return int.MaxValue;
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        if ( !a.isFloor && !b.isFloor)
        {
            if (b.height > jumpHeight)
            {
                return 10000000;
            }
            remaining += 30;
        }
        if (!b.isFloor)
        {
            remaining += 20;
        }
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodesList)
    {
        PathNode lowestFCostNode = pathNodesList[0];
        for (int i = 1; i < pathNodesList.Count; i++)
        {
            if (pathNodesList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodesList[i];
            }
        }

        return lowestFCostNode;
    }

    public List<PathNode> FindPathForPhysic(Vector3 start, Vector3 end, float jumpHeight)
    {
        PathNode startNode = _grid.GetValue(start);
        PathNode endNode = _grid.GetValue(end);

        if (endNode == null || startNode == null) return null;  

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();
        
        foreach (var tilePosition in _emptyTilePositions)
        {
            PathNode node = _grid.GetValue(tilePosition);
            if (node == null) continue;
            node.gCost = int.MaxValue;
            node.CalculateFCost();
            node.cameFromNode = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode) || neighbourNode == null) continue;

                var CDCP = CalculateDistanceCostPhysic(currentNode, neighbourNode, jumpHeight);
                int tentativeGCost = currentNode.gCost + CDCP;
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        
        // Out of nodes on the openList
        Debug.Log("Doesn't work");
        return null;
    }

    public Grid<PathNode> GetGrid()
    {
        return _grid;
    }

    public bool IsCellEmpty(PathNode pathNode)
    {
        var pos = pathNode.GetPosition();
        return _emptyTilePositions.Contains(new Vector3Int((int)pos.x, (int)pos.y));
    }
}
