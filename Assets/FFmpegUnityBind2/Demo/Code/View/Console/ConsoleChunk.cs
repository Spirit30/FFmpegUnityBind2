using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class ConsoleChunk : MonoBehaviour
    {
        [SerializeField]
        Text lable = null;

        public int Length { get; private set; }

        public float Height => RectTransform.sizeDelta.y;

        RectTransform rectTransform;
        RectTransform RectTransform
        {
            get
            {
                if (!rectTransform)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        public void Append(string text)
        {
            Length += text.Length;
            lable.text += text;
        }
    }
}