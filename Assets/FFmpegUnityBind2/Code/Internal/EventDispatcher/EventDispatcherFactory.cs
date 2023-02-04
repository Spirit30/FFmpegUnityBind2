using UnityEngine;

namespace FFmpegUnityBind2.Internal
{
    static class EventDispatcherFactory
    {
        static EventDispatcher instance;

        public static EventDispatcher GetInstance()
        {
            return instance;
        }

        public static void CreateInstance()
        {
            instance = new GameObject(typeof(EventDispatcher).Name).AddComponent<EventDispatcher>();
        }
    }
}
