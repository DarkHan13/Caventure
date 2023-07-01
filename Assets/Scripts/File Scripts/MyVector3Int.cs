using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class MyVector3Int
{
    private static int dx = 0;

    [JsonProperty("X")]
    public int x { get; set; }
    
    [JsonProperty("Y")]
    public int y { get; set; }
    
    [JsonProperty("Z")]
    public int z { get; set; }

    public MyVector3Int()
    {
        
    }
    public MyVector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("x", this.x);
        info.AddValue("y", this.y);
        info.AddValue("z", this.z);
    }

    public Vector3Int GetVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
    
    public override int GetHashCode()
    {
        int hashCode1 = y.GetHashCode();
        int hashCode2 = z.GetHashCode();
        return x.GetHashCode() ^ hashCode1 << 4 ^ hashCode1 >> 28 ^ hashCode2 >> 4 ^ hashCode2 << 28;
    }

    public override bool Equals(object obj)
    {
        MyVector3Int o = (MyVector3Int)obj;
        return x == o.x && y == o.y && z == o.z;
    }

    public static MyVector3Int operator +(MyVector3Int v1, MyVector3Int v2)
    {
        return new MyVector3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }
    
    public static readonly MyVector3Int Up = new(0, 1, 0);
    public static readonly MyVector3Int Down = new(0, -1, 0);
    public static readonly MyVector3Int Left = new(-1, 0, 0);
    public static readonly MyVector3Int Right = new(1, 0, 0);
    public static readonly MyVector3Int LeftUp = new(-1, 1, 0);
    public static readonly MyVector3Int LeftDown = new(-1, -1, 0);
    public static readonly MyVector3Int RightUp = new(1, 1, 0);
    public static readonly MyVector3Int RightDown = new(1, -1, 0);


    public override string ToString()
    {
        return x + " " + y + " " + z;
    }
}
