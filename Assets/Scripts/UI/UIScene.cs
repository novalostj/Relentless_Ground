using SceneScriptFolder;
using UnityEngine;

namespace UI
{
    public class UIScene : MonoBehaviour
    {
        [SerializeField] private SceneScriptableObject sceneScriptableObject;

        public void LoadScene(string sceneName)
        {
            MainSceneManager.loadScene?.Invoke(sceneName);
        }
    }
}