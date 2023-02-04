using UnityEngine;
using UnityEngine.UI;

namespace GigaVideoPlayer.Demo
{
    class GigaVideoPlayerDemo : MonoBehaviour
    {
        [SerializeField]
        VideoSource videoSource = VideoSource.BuiltInClip;

        [SerializeField]
        GigaVideoPlayerFromClipDemo fromClipDemo = null;

        [SerializeField]
        GigaVideoPlayerFromPathDemo fromPathDemo = null;

        [SerializeField]
        GigaVideoPlayerFromUrlDemo fromUrlDemo = null;

        [SerializeField]
        bool loop = false;

        [SerializeField]
        ColorSheme colorSheme = null;

        [SerializeField]
        Button playButton = null;

        VideoPlayerWrapper activeVideoPlayer;

        void Awake()
        {
            playButton.onClick.AddListener(OnPlayButton);
        }

        void Update()
        {
            if (activeVideoPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    activeVideoPlayer.Play(!activeVideoPlayer.IsPlay);
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    activeVideoPlayer.Stop();
                }
            }
        }

        void OnPlayButton()
        {
            if(videoSource == VideoSource.BuiltInClip)
            {
                fromClipDemo.TryPlay();
                activeVideoPlayer = fromClipDemo.ActiveVideoPlayer;
            }
            else if (videoSource == VideoSource.Path)
            {
                fromPathDemo.TryPlay();
                activeVideoPlayer = fromPathDemo.ActiveVideoPlayer;
            }
            else if (videoSource == VideoSource.Url)
            {
                fromUrlDemo.TryPlay();
                activeVideoPlayer = fromUrlDemo.ActiveVideoPlayer;
            }

            SetupActiveVideoPlayer();
        }

        void SetupActiveVideoPlayer()
        {
            if (activeVideoPlayer)
            {
                activeVideoPlayer.IsLoop = loop;
                activeVideoPlayer.Theme.SetScheme(colorSheme);
            }
        }
    }
}