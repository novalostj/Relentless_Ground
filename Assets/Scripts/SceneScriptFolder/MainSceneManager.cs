using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneScriptFolder
{
    [Serializable]
    public class LocalScene
    {
        public string name;

        public bool isLoaded;
    }
    
    
    
    [Serializable]
    public class GroupOfScene
    {
        public List<LocalScene> scenes;

        public void LoadAsync()
        {
            foreach (var scene in scenes)
            {
                if (scene.isLoaded) continue;
                
                SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
                scene.isLoaded = true;
            }
                    
        }

        public void UnloadAll()
        {
            foreach (var scene in scenes)
            {
                if (!scene.isLoaded) continue;
                
                SceneManager.UnloadSceneAsync(scene.name);
                scene.isLoaded = false;
            }
        }
    }
    
    
    public class MainSceneManager : MonoBehaviour
    {
        public List<GroupOfScene> sceneGroup;

        private void Start()
        {
            sceneGroup[0].LoadAsync();
        }
    }
}