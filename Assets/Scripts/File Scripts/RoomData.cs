using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class RoomData
{
    public string name;
    [JsonProperty("GroundTiles")]
    public Dictionary<MyVector3Int, int> GroundTiles { get; set; }
    [JsonProperty("Walls")]
    public Dictionary<MyVector3Int, int> Walls { get; set; }

    [JsonProperty("Platforms")] 
    public Dictionary<MyVector3Int, int> Platforms { get; set; }

    public RoomData()
    {
        GroundTiles = new Dictionary<MyVector3Int, int>(50);
        Walls = new Dictionary<MyVector3Int, int>(50);
        Platforms = new Dictionary<MyVector3Int, int>(10);
    }

    public static void RemoveAdjacentTiles( Dictionary<MyVector3Int, int> data, MyVector3Int pos)
    {
        if (data.ContainsKey(pos))
        {
            data.Remove(pos);
        }

        MyVector3Int tmp = pos + MyVector3Int.LeftUp;
        if (data.ContainsKey(tmp))
        {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.Up;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.RightUp;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.Left;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.Right;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.LeftDown;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
        tmp = pos + MyVector3Int.Down;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        tmp = pos + MyVector3Int.RightDown;
        if (data.ContainsKey(tmp)) {
            RemoveAdjacentTiles(data, tmp);
        }
        
    }
}
