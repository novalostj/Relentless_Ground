using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Utility
{
    public static class Save<T>
    {
        public static void CreateOrOverwrite(string savePath, T saveFile)
        {
            string saveData = JsonUtility.ToJson(saveFile, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            bf.Serialize(file, saveData);
            file.Close();
            
        }
        
        public static void Load(string savePath, T saveFile)
        {
            if (!File.Exists(string.Concat(Application.persistentDataPath, savePath))) return;
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), saveFile);
            file.Close();
        }
    }

    public static class Save
    {
        public static void SaveTexture2D(Texture2D texture, string fileName)
        {
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = $"{Application.persistentDataPath}/{fileName}";

            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            
            Debug.Log(dirPath);
            
            File.WriteAllBytes($"{dirPath}/_Image.png", bytes);
        }
        
        public static void SaveTexture2D(Texture2D texture, string fileName, int index)
        {
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = $"{Application.persistentDataPath}/{fileName}";

            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            
            Debug.Log(dirPath);
            
            File.WriteAllBytes($"{dirPath}/image_{index}.png", bytes);
        }
    }
}