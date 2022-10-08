using System;
using System.Collections.Generic;
using Stats;
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
    public class LocalGroupScene
    {
        public string name;
        public List<LocalScene> scenes;

        public bool isLoaded { get; private set; }
        
        
        public void LoadAsync()
        {
            isLoaded = true;
            
            foreach (var scene in scenes)
            {
                if (scene.isLoaded) continue;
                
                SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
                scene.isLoaded = true;
            }
        }

        public void UnloadAll()
        {
            isLoaded = false;
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
        public delegate void LoadScene<in TT>(TT name);
        public static LoadScene<string> loadScene;

        [SerializeField] private SceneScriptableObject sceneScriptableObject;
        
        private List<LocalGroupScene> SceneGroup => sceneScriptableObject.sceneGroup;

        private LocalGroupScene CurrentMainScene { 
            get => sceneScriptableObject.CurrentMainScene;
            set => sceneScriptableObject.CurrentMainScene = value;
        }


        private void OnEnable()
        {
            loadScene += LoadLocalGroupScene;
            PlayerStatus.noHealth += GoToMainMenu;
        }

        private void OnDisable()
        {
            loadScene -= LoadLocalGroupScene;
            PlayerStatus.noHealth -= GoToMainMenu;
        }

        private void Start()
        {
            GoToMainMenu();
        }

        private void LoadLocalGroupScene(LocalGroupScene localGroupScene)
        {
            if (localGroupScene.isLoaded) return;


            Debug.Log("Null3");
            CurrentMainScene?.UnloadAll();

            CurrentMainScene = localGroupScene;
            
            CurrentMainScene.LoadAsync();
        }

        private void UnloadCurrentScene(LocalGroupScene localGroupScene)
        {
            localGroupScene.UnloadAll();
        }

        private LocalGroupScene FindGroupScene(string methodName)
        {
            foreach (var localGroupScene in SceneGroup)
                if (localGroupScene.name == methodName)
                    return localGroupScene;

            Debug.Log("Null2");
            return null;
        }

        private void LoadLocalGroupScene(string methodName)
        {
            Debug.Log("Null1");
            LoadLocalGroupScene(FindGroupScene(methodName));
        }

        private void GoToMainMenu()
        {
            LoadLocalGroupScene("Main Menu");
        }

        private void OnApplicationQuit()
        {
            CurrentMainScene = null;
            
            foreach (var localGroupScene in SceneGroup)
            {
                localGroupScene.UnloadAll();
            }
        }
    }
}