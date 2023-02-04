using System.Collections.Generic;
using UnityEngine;

namespace FFmpegUnityBind2.Internal
{
    class EventDispatcher : MonoBehaviour
    {
        static readonly Queue<Event> events = new Queue<Event>();

        public static void Invoke(Event @event)
        {
            events.Enqueue(@event);
        }

        static void TryExecute()
        {
            if (events.Count > 0)
            {
                events.Dequeue()?.Handle();
            }
        }

        void Update()
        {
            TryExecute();
        }

        void OnDestroy()
        {
            TryExecute();
        }
    }
}
