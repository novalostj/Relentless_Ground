using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static System.IO.File;
using static UnityEngine.Application;
using static UnityEngine.JsonUtility;

namespace Utility
{
    public static class Save<T>
    {
        public static void CreateOrOverwrite(string savePath, T saveFile)
        {
            var saveData = ToJson(saveFile, true);
            var bf = new BinaryFormatter();
            var file = Create(string.Concat(persistentDataPath, savePath));
            bf.Serialize(file, saveData);
            file.Close();
        }
        
        public static void Load(string savePath, T saveFile)
        {
            if (!Exists(string.Concat(persistentDataPath, savePath))) return;
            
            var bf = new BinaryFormatter();
            var file = Open(string.Concat(persistentDataPath, savePath), FileMode.Open);
            FromJsonOverwrite(bf.Deserialize(file).ToString(), saveFile);
            file.Close();
        }
    }
    
    public static class Save
    {
        public static void SaveTexture2D(Texture2D texture, string fileName)
        {
            var bytes = texture.EncodeToPNG();
            var dirPath = $"{persistentDataPath}/{fileName}";

            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            Debug.Log(dirPath);
            
            WriteAllBytes($"{dirPath}/_Image.png", bytes);
        }
        
        public static void SaveTexture2D(Texture2D texture, string fileName, int index)
        {
            var bytes = texture.EncodeToPNG();
            var dirPath = $"{persistentDataPath}/{fileName}";

            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            Debug.Log(dirPath);
            
            WriteAllBytes($"{dirPath}/image_{index}.png", bytes);
        }
    }
}