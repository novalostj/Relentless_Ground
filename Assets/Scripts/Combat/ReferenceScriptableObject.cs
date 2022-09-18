using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Enemy_Reference", menuName = "Enemy/Target References")]
    public class ReferenceScriptableObject : ScriptableObject
    {
        public GameObject target;
    }
}