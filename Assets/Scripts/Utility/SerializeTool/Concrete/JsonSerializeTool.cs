using System.IO;
using UnityEngine;

namespace Utility.SerializeTool.Concrete
{
    public class JsonSerializeTool : ISerializeTool
    {
        public void Serialize<T>(T data, string path)
        {
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
        }

        public T Deserialize<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
    }
}