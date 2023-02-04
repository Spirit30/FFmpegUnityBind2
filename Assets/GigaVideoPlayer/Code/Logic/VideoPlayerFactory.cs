using UnityEngine;
using UnityEngine.EventSystems;

namespace GigaVideoPlayer.Internal
{
    static class VideoPlayerFactory
    {
        public static VideoPlayerWrapper Instantiate()
        {
            var instance = Object.Instantiate(LoadPrefab());
            instance.Theme = Object.Instantiate(instance.ThemePrefab, instance.transform);
            if(!EventSystem.current)
            {
                new GameObject(typeof(EventSystem).Name, typeof(EventSystem), typeof(StandaloneInputModule));
            }
            return instance;
        }

        static VideoPlayerWrapper LoadPrefab()
        {
            return Resources.Load<VideoPlayerWrapper>("GigaVideoPlayer");
        }
    }
}