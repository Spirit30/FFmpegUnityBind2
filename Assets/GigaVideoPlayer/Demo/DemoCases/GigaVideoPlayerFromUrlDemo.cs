using System;
using UnityEngine;

namespace GigaVideoPlayer.Demo
{
    [Serializable]
    class GigaVideoPlayerFromUrlDemo
    {
        [SerializeField]
        string url = string.Empty;

        public VideoPlayerWrapper ActiveVideoPlayer { get; private set; }

        //With Validation.
        public void TryPlay()
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (IsUrlValid(url))
                {
                    if (ActiveVideoPlayer)
                    {
                        ActiveVideoPlayer.PlayFromUrl(url);
                    }
                    else
                    {
                        ActiveVideoPlayer = Video.PlayFromUrl(url);
                    }
                }
                else
                {
                    Debug.LogError($"Invalid url: {url}.");
                }
            }
            else
            {
                Debug.LogError("Please set url.");
            }
        }

        static bool IsUrlValid(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
