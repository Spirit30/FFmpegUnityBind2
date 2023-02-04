using GigaVideoPlayer.Internal;
using System;
using UnityEngine.Video;

namespace GigaVideoPlayer
{
    public static class Video
    {
        public static VideoPlayerWrapper PlayFromClip(VideoClip clip)
        {
            var videoPlayer = VideoPlayerFactory.Instantiate();
            videoPlayer.PlayFromClip(clip);
            return videoPlayer;
        }

        public static VideoPlayerWrapper PlayFromPath(string path)
        {
            return PlayFromUri(path);
        }

        public static VideoPlayerWrapper PlayFromUrl(string url)
        {
            return PlayFromUri(url);
        }

        public static VideoPlayerWrapper PlayFromUri(Uri uri)
        {
            return PlayFromUri(uri.ToString());
        }

        public static VideoPlayerWrapper PlayFromUri(string uri)
        {
            var videoPlayer = VideoPlayerFactory.Instantiate();
            videoPlayer.PlayFromUri(uri);
            return videoPlayer;
        }
    }
}