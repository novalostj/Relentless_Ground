using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAnimation : MonoBehaviour
    {
        [Header("Settings Animation")]
        public RectTransform settingsRectTransform;
        public float startY, endY;
        public float animationTime = 1f;
        public float timeMultiplier;
        public bool isEnabled;
        public Image title;

        private Coroutine moveUp, moveDown;
        
        public void Toggle()
        {
            isEnabled = !isEnabled;

            if (isEnabled)
            {
                if (moveDown != null)
                    StopCoroutine(moveDown);
                moveUp = StartCoroutine(MoveUp());
            }
            else
            {
                if (moveUp != null)
                    StopCoroutine(moveUp);
                moveDown = StartCoroutine(MoveDown());
            }
        }

        private IEnumerator MoveUp()
        {
            float localTime = 0;
            
            while (true)
            {
                yield return new WaitForEndOfFrame();
                localTime += Time.deltaTime * timeMultiplier;
                settingsRectTransform.anchoredPosition = new Vector2(settingsRectTransform.anchoredPosition.x, Mathf.Lerp(startY, endY, localTime));
                title.color = new Color(title.color.r, title.color.g, title.color.b, animationTime - localTime);
                
                if (localTime <= 0)
                    break;
            }
        }

        private IEnumerator MoveDown()
        {
            float localTime = 0;
            
            while (true)
            {
                yield return new WaitForEndOfFrame();
                localTime += Time.deltaTime * timeMultiplier;
                settingsRectTransform.anchoredPosition = new Vector2(settingsRectTransform.anchoredPosition.x, Mathf.Lerp(endY, startY, localTime));
                title.color = new Color(title.color.r, title.color.g, title.color.b, localTime);
                
                if (localTime <= 0)
                    break;
            }
        }
    }
}