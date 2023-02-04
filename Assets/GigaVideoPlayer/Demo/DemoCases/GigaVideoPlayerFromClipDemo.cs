using System;
using UnityEngine;
using UnityEngine.Video;

namespace GigaVideoPlayer.Demo
{
    [Serializable]
    class GigaVideoPlayerFromClipDemo
    {
        [SerializeField]
        VideoClip videoClip = null;

        public VideoPlayerWrapper ActiveVideoPlayer { get; private set; }

        //With Validation.
        public void TryPlay()
        {
            if (videoClip)
            {
                if (ActiveVideoPlayer)
                {
                    ActiveVideoPlayer.PlayFromClip(videoClip);
                }
                else
                {
                    ActiveVideoPlayer = Video.PlayFromClip(videoClip);
                }
            }
            else
            {
                Debug.LogError("Please assign Video Clip.");
            }
        }
    }
}