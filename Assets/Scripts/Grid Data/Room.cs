using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : IComparable<Room>
{
    public MyVector3Int position;
    public List<Room> connectedRooms;
    public int roomSize;
    public BoundsInt bounds;
    public bool IsMainRoom;
    public bool IsAccessibleFromMainRoom;
    public RoomData RoomData;

    public Room() : this(0, 0){ }
    public Room(int x, int y)
    {
        position = new MyVector3Int(x, y, 0);
        RoomData data = FileIOService.NewLoadData(Application.persistentDataPath + "/test.txt");
        


        RoomData = new RoomData();
        RoomData.name = data.name;

        FillData(data.GroundTiles, RoomData.GroundTiles);
        FillData(data.Walls, RoomData.Walls);
        FillData(data.Platforms, RoomData.Platforms);
        RecalculateBounds(RoomData.GroundTiles);
        
        
        roomSize = bounds.size.x * bounds.size.y;
        connectedRooms = new List<Room>();
    }

    public void ChangePosition(int x, int y)
    {
        position = new MyVector3Int(x, y, 0);
        
        Dictionary<MyVector3Int, int> tmp = new Dictionary<MyVector3Int, int>();
        
        FillData(RoomData.GroundTiles, tmp);
        RoomData.GroundTiles = new Dictionary<MyVector3Int, int>(tmp);
        tmp.Clear();
        
        FillData(RoomData.Walls, tmp);
        RoomData.Walls = new Dictionary<MyVector3Int, int>(tmp);
        tmp.Clear();
        
        FillData(RoomData.Platforms, tmp);
        RoomData.Platforms = new Dictionary<MyVector3Int, int>(tmp);
        tmp.Clear();
        
        RecalculateBounds(RoomData.GroundTiles);
    }
   

    void FillData(Dictionary<MyVector3Int, int> data, Dictionary<MyVector3Int, int> res)
    {
        foreach (var (key, value) in data)
        {
            res.Add(key + position, value);
        }
    }

    void RecalculateBounds(Dictionary<MyVector3Int, int> data)
    {
        int minX = Int32.MaxValue;
        int minY = Int32.MaxValue;
        int maxX = Int32.MinValue;
        int maxY = Int32.MinValue;
        
        foreach (var (pos, value) in data)
        {
            if (minX > pos.x) minX = pos.x;
            if (maxX < pos.x) maxX = pos.x;
            if (minY > pos.y) minY = pos.y;
            if (maxY < pos.y) maxY = pos.y;
        }
        bounds = new BoundsInt(minX, minY, 0, maxX - minX, maxY - minY, 0);
        bounds.xMin = minX;
        bounds.xMax = maxX;
    }
    public void SetAccessibleFromMainRoom()
    {
        if (!IsAccessibleFromMainRoom)
        {
            IsAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }
    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.IsAccessibleFromMainRoom)
        {
            roomB.SetAccessibleFromMainRoom();
        } else if (roomB.IsAccessibleFromMainRoom)
        {
            roomA.SetAccessibleFromMainRoom();
        }
        roomB.connectedRooms.Add(roomA);
        roomA.connectedRooms.Add(roomB);
    }

    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }

    public bool IsCross(int x1, int y1, int x2, int y2)
    {
        int x5 = Math.Max(x1, position.x);
        int y5 = Math.Max(y1, position.y);
        int x6 = Math.Min(x2, position.x + bounds.size.x);
        int y6 = Math.Min(y2, position.y + bounds.size.y);
        return x5 <= x6 && y5 <= y6;
    }
    
    // not tested
    public bool IsCross(Vector2Int leftDown, int width, int height)
    {
        int dx = Math.Abs(leftDown.x - position.x);
        int dy = Math.Abs(leftDown.y - position.y);
        int dw = Math.Abs(width - bounds.size.x);
        int dh = Math.Abs(height - bounds.size.y);
        return dx <= dw && dy <= dh;
    }

    public int CompareTo(Room other)
    {
        return other.roomSize.CompareTo(roomSize);
    }
}
