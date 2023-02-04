using GigaVideoPlayer.Internal;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace GigaVideoPlayer
{
    public class VideoPlayerWrapper : MonoBehaviour
    {
        [SerializeField]
        Canvas canvas = null;

        [SerializeField]
        Theme themePrefab = null;

        [SerializeField]
        VideoPlayer videoPlayer = null;

        [SerializeField]
        AudioSource audioSource = null;

        public VideoPlayer VideoPlayer => videoPlayer;

        public bool UriSource
        {
            get => VideoPlayer.source == UnityEngine.Video.VideoSource.Url;
            set => VideoPlayer.source = value ? UnityEngine.Video.VideoSource.Url : UnityEngine.Video.VideoSource.VideoClip;
        }

        public string Uri
        {
            get => videoPlayer.url;
            set => videoPlayer.url = value;
        }

        public VideoClip Clip
        {
            get => videoPlayer.clip;
            set => videoPlayer.clip = value;
        }

        public float Progress
        {
            get => VideoPlayer.length > 0 ? (float)VideoPlayer.time / (float)VideoPlayer.length : 0;
            set
            {
                if (VideoPlayer.canSetTime)
                {
                    VideoPlayer.time = value * VideoPlayer.length;
                    theme.SetProgress(value);
                }
            }
        }

        public float Volume
        {
            get => audioSource.volume;
            set
            {
                audioSource.volume = value;
                theme.SetVolume(value);
                theme.SetSoundOnOffButtonState(value > 0);
            }
        }

        public Texture Texture => VideoPlayer.texture;

        public bool IsPlay => VideoPlayer.isPlaying;

        public string FileName =>
            VideoPlayer.source == UnityEngine.Video.VideoSource.Url
                ? Path.GetFileName(VideoPlayer.url)
                : VideoPlayer.clip
                    ? VideoPlayer.clip.name
                    : string.Empty;

        public Theme ThemePrefab => themePrefab;

        Theme theme;
        public Theme Theme
        {
            get => theme;
            set
            {
                if (theme)
                {
                    DestroyImmediate(theme.gameObject);
                }
                theme = value;
                theme.VideoPlayer = this;
            }
        }

        public bool IsActive
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        public bool IsVisible
        {
            get => canvas.enabled;
            set => canvas.enabled = value;
        }

        public int Depth
        {
            get => canvas.sortingOrder;
            set => canvas.sortingOrder = value;
        }

        public bool IsLoop
        {
            get => videoPlayer.isLooping;
            set => videoPlayer.isLooping = value;
        }

        public event Action OnEnd = delegate { };

        public void PlayFromClip(VideoClip clip)
        {
            InitAudio();
            UriSource = false;
            Clip = clip;
            Play(true);
        }

        public void PlayFromPath(string path)
        {
            PlayFromUri(path);
        }

        public void PlayFromUrl(string url)
        {
            PlayFromUri(url);
        }

        public void PlayFromUri(Uri uri)
        {
            PlayFromUri(uri.ToString());
        }

        public void PlayFromUri(string uri)
        {
            InitAudio();
            UriSource = true;
            Uri = uri;
            Play(true);
        }

        public void Play(bool value)
        {
            Enable(true);

            if (value)
            {
                VideoPlayer.Play();
            }
            else
            {
                VideoPlayer.Pause();
            }

            theme.SetPlayPauseButtonState(value);
        }

        public void Stop()
        {
            VideoPlayer.Stop();
            Enable(false);
        }

        void Enable(bool flag)
        {
            IsActive = flag;
            IsVisible = flag;
        }

        void InitAudio()
        {
            VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            VideoPlayer.controlledAudioTrackCount = 1;
            VideoPlayer.SetTargetAudioSource(0, audioSource);
            VideoPlayer.EnableAudioTrack(0, true);
        }

        void Awake()
        {
            VideoPlayer.loopPointReached += OnEndInternal;
            VideoPlayer.errorReceived += OnErrorReceived;
        }

        void Update()
        {
            theme.SetFileName(FileName);
            theme.SetProgress(Progress);
        }

        void OnEndInternal(VideoPlayer videoPlayer)
        {
            if(!IsLoop)
            {
                Stop();
                OnEnd();
            }
        }

        void OnErrorReceived(VideoPlayer source, string message)
        {
            string error = $"Video Player Error.\nPlease check {Application.platform} supported formats.\n{message}";
            Debug.LogError(error);
            Theme.Error = error;
        }
    }
}