using System.Collections.Generic;
using UnityEngine;

namespace SceneScriptFolder
{
    [CreateAssetMenu(fileName = "Scene Management Scriptable", menuName = "Scene Management")]
    public class SceneScriptableObject : ScriptableObject
    {
        public List<LocalGroupScene> sceneGroup;
        public LocalGroupScene CurrentMainScene { get; set; }
    }
}