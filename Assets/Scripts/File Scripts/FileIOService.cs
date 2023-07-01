using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class FileIOService
{
    private readonly string PATH;

    public FileIOService(string path)
    {
        PATH = path;
    }

    private static Object[] files;

    public static Dictionary<MyVector3Int, int> LoadData(string path)
    {
        var fileExists = File.Exists(path);
        if (!fileExists)
        {
            File.CreateText(path).Dispose();
            return null;
        }
    
        using (var reader = File.OpenText(path))
        {
            var fileText = reader.ReadToEnd();
            Dictionary<string, int> tmp = JsonConvert.DeserializeObject<Dictionary<string, int>>(fileText);
            Dictionary<MyVector3Int, int> result = new Dictionary<MyVector3Int, int>();
            foreach (var (key, value) in tmp)
            {
                //result.Add(new MyVector3Int(key), value);
            }

            return result;
        }
        
        
    }

    public static void SaveData(Dictionary<MyVector3Int, int> data, string path)
    {
        Debug.Log(path);
        using (StreamWriter writer = File.CreateText(path))
        {
            string output = JsonConvert.SerializeObject(data);
            writer.Write(output);
        }
    }
    
    public static RoomData NewLoadData(string path)
    {

        Debug.Log("Find Rooms...");
        string fileName = "default";
        string text = (Resources.Load("Rooms/room_1") as TextAsset).text;
        if (files == null) files = Resources.LoadAll("Rooms/", typeof(TextAsset));
        
        if (files != null && files.Length > 0)
        {
            TextAsset txt = (TextAsset)files[Random.Range(0, files.Length)];
            text = txt.text;
            fileName = txt.name;
        }
        RoomData tmp = JsonConvert.DeserializeObject<RoomData>(text, new MyConverter());
        tmp.name = fileName;
        return tmp;



    }
    public static void NewSaveData(RoomData data, string path)
    {
        using (StreamWriter writer = File.CreateText(path))
        {
            string output = JsonConvert.SerializeObject(data, new MyConverter());
            writer.Write(output);
        }
    }
}
