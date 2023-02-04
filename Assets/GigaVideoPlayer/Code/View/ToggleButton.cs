using UnityEngine;
using UnityEngine.UI;

namespace GigaVideoPlayer
{
    class ToggleButton : MonoBehaviour
    {
        [SerializeField]
        Button button = null;

        [SerializeField]
        Image icon = null;

        [SerializeField]
        Sprite onSprite = null;

        [SerializeField]
        Sprite offSprite = null;

        [SerializeField]
        bool isOn = false;

        public bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                UpdateView();
            }
        }

        public Button Button => button;

        void UpdateView()
        {
            icon.sprite = IsOn ? onSprite : offSprite;
        }
    }
}