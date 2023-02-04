using UnityEngine;

namespace GigaVideoPlayer
{
    public static class RectUtil
    {
        public static void SetRect(this RectTransform rectTransform, Rect rect)
        {
            rectTransform.anchorMin = new Vector2(rect.position.x / Screen.width, rect.position.y / Screen.height);
            rectTransform.anchorMax = new Vector2((rect.position.x + rect.width) / Screen.width, (rect.position.y + rect.height) / Screen.height);
            rectTransform.anchoredPosition =
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}
