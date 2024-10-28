using System.IO;
using UnityEngine;

namespace Utility.SerializeTool
{
    public interface ISerializeTool
    {
        public void Serialize<T>(T data, string path);
        public T Deserialize<T>(string path);
    }
}