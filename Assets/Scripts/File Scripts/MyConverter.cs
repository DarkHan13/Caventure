using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MyConverter : JsonConverter
{
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Dictionary<MyVector3Int, int>)
        {
            Dictionary<MyVector3Int, int> dic = (Dictionary<MyVector3Int, int>)value;
            writer.WriteStartArray();
            foreach (var (key, i) in dic)
            {
                serializer.Serialize(writer, key);
                serializer.Serialize(writer, i);
            }
            writer.WriteEndArray();
            return;
        }
        writer.WriteStartObject();
        writer.WriteEndObject();
    
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            JArray jArray = JArray.Load(reader);
            Dictionary<MyVector3Int, int> dic = new Dictionary<MyVector3Int, int>();
            if (jArray == null || jArray.Count < 2) return dic;
            
            for (int i = 0; i < jArray.Count; i += 2)
            {
                string key = jArray[i].ToString();
                string value = jArray[i + 1].ToString();
                MyVector3Int vec = JsonConvert.DeserializeObject<MyVector3Int>(key);
                int integer = JsonConvert.DeserializeObject<int>(value);
                if (!dic.ContainsKey(vec)) dic.Add(vec, integer);
            }
            return dic;
        }

        return new Dictionary<MyVector3Int, int>();
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(Dictionary<MyVector3Int, int>).IsAssignableFrom(objectType);
    }
}
