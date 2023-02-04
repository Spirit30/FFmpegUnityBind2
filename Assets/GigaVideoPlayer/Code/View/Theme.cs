using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GigaVideoPlayer.Internal
{
    public class Theme : MonoBehaviour
    {
        [SerializeField]
        VideoPlayerWrapper videoPlayer = null;

        [SerializeField]
        Button canvasButton = null;

        [SerializeField]
        Button overlayButton = null;

        [SerializeField]
        Animator overlay = null;

        [SerializeField]
        float hideOverlayTimeSec = 3.0f;

        [SerializeField]
        ToggleButton[] playPauseButtons = null;

        [SerializeField]
        ToggleButton soundOnOffButton = null;

        [SerializeField]
        ToggleButton minimizeMaximizeButton = null;

        [SerializeField]
        Slider progressSlider = null;

        [SerializeField]
        Slider soundSlider = null;

        [SerializeField]
        Text errorLable = null;

        [SerializeField]
        Text[] nameLables = null;

        [SerializeField]
        Graphic[] graphicsA = null;

        [SerializeField]
        Graphic[] graphicsB = null;

        [SerializeField]
        ColorSheme defaultColorScheme = null;

        public VideoPlayerWrapper VideoPlayer
        {
            get => videoPlayer;
            set => videoPlayer = value;
        }

        public bool CanMinimize
        {
            get => minimizeMaximizeButton.gameObject.activeSelf;
            set => minimizeMaximizeButton.gameObject.SetActive(value);
        }

        public string Error
        {
            get => errorLable.text;
            set => errorLable.text = value;
        }

        public RectTransform RectTransform => (RectTransform)transform;
        public Rect MinimizeRect { get; set; } = new Rect(Screen.width * 0.025f, Screen.height - Screen.height * 0.33f - Screen.height * 0.025f, Screen.width * 0.33f, Screen.height * 0.33f);
        public static Rect MaximizeRect { get; } = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

        Dictionary<Graphic, Color> defaultColors = new Dictionary<Graphic, Color>();

        float previousVolume;
        string nameFormat;
        string colorHTML;
        bool isOverlayVisible;
        float lastUpdateTime;

        public void SetScheme(ColorSheme scheme)
        {
            SetColorA(scheme.colorA);
            SetColorB(scheme.colorB);
        }

        public void SetColorA(Color color)
        {
            colorHTML = ColorUtility.ToHtmlStringRGB(color);

            SetColor(graphicsA, color);
        }

        public void SetColorB(Color color)
        {
            SetColor(graphicsB, color);
        }

        public void SetPlayPauseButtonState(bool value)
        {
            foreach (var playPauseButton in playPauseButtons)
            {
                playPauseButton.IsOn = value;
            }
        }

        public void SetSoundOnOffButtonState(bool value)
        {
            soundOnOffButton.IsOn = value;
        }

        public void SetMinimizeButtonState(bool value)
        {
            minimizeMaximizeButton.IsOn = value;

            nameLables.First().enabled = value;
            nameLables.Last().enabled = !value;
        }

        public void SetProgress(float value)
        {
            progressSlider.SetValueWithoutNotify(value);
        }

        public void SetVolume(float value)
        {
            soundSlider.SetValueWithoutNotify(value);
        }

        public void SetFileName(string fileName)
        {
            foreach(var nameLable in nameLables)
            {
                nameLable.text = string.Format(nameFormat, colorHTML, fileName);
            }
        }

        public void Minimize(bool value)
        {
            if(CanMinimize)
            {
                RectTransform.SetRect(value ? MinimizeRect : MaximizeRect);

                SetMinimizeButtonState(value);
            }
        }

        public void UpdateView()
        {
            SetPlayPauseButtonState(videoPlayer.IsPlay);
            SetSoundOnOffButtonState(videoPlayer.Volume > 0);
            SetMinimizeButtonState(true);
            SetProgress(videoPlayer.Progress);
            SetVolume(videoPlayer.Volume);
        }

        void SetColor(Graphic[] graphics, Color color)
        {
            foreach(var graphic in graphics)
            {
                if(!defaultColors.ContainsKey(graphic))
                {
                    defaultColors.Add(graphic, graphic.color);
                }

                var defaultColor = defaultColors[graphic];
                graphic.color = defaultColor * color;
            }
        }

        void EnableOverlay(bool flag)
        {
            overlay.SetTrigger(flag ? "Show" : "Hide");
            isOverlayVisible = flag;
        }

        void ResetUpdateTime()
        {
            lastUpdateTime = Time.time;
        }

        void Awake()
        {
            canvasButton.onClick.AddListener(OnCanvasButton);
            overlayButton.onClick.AddListener(ResetUpdateTime);

            foreach (var playPauseButton in playPauseButtons)
            {
                playPauseButton.Button.onClick.AddListener(OnPlayPauseButton);
            }

            soundOnOffButton.Button.onClick.AddListener(OnSoundOnOffButton);
            minimizeMaximizeButton.Button.onClick.AddListener(OnMinimizeMaximizeButton);
            progressSlider.onValueChanged.AddListener(OnChangeProgressSlider);
            soundSlider.onValueChanged.AddListener(OnChangeSoundSlider);
            nameFormat = nameLables.First().text;
            SetScheme(defaultColorScheme);
            EnableOverlay(true);
        }

        void OnEnable()
        {
            if(videoPlayer)
            {
                UpdateView();
            }

            ResetUpdateTime();
        }

        void Update()
        {
            if(isOverlayVisible && (Time.time - lastUpdateTime) > hideOverlayTimeSec)
            {
                EnableOverlay(false);
            }
        }

        void OnDisable()
        {
            Error = string.Empty;
        }

        #region UI EVENTS

        void OnCanvasButton()
        {
            EnableOverlay(true);
            ResetUpdateTime();
        }

        void OnPlayPauseButton()
        {
            bool value = !playPauseButtons.First().IsOn;

            videoPlayer.Play(value);

            ResetUpdateTime();
        }

        void OnSoundOnOffButton()
        {
            bool value = !soundOnOffButton.IsOn;

            if(value)
            {
                videoPlayer.Volume = previousVolume;
            }
            else
            {
                previousVolume = videoPlayer.Volume;
                videoPlayer.Volume = 0;
            }

            ResetUpdateTime();
        }

        void OnMinimizeMaximizeButton()
        {
            Minimize(!minimizeMaximizeButton.IsOn);

            ResetUpdateTime();
        }

        void OnChangeProgressSlider(float value)
        {
            videoPlayer.Progress = value;

            ResetUpdateTime();
        }

        void OnChangeSoundSlider(float value)
        {
            videoPlayer.Volume = value;

            ResetUpdateTime();
        }

        #endregion
    }
}