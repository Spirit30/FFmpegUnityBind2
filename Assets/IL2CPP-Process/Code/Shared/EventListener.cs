using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IL2CPP.Process.Shared
{
    class EventListener
    {
        readonly DirectoryInfo serverDirectory;
        readonly EventType[] listenTypes;

        public event Action<List<Event>> OnEvents = delegate { };

        public EventListener(DirectoryInfo serverDirectory, CancellationToken cancelationToken, params EventType[] listenTypes)
        {
            this.serverDirectory = serverDirectory;
            this.listenTypes = listenTypes;

            Task.Run(async () =>
            {
                try
                {
                    while (!cancelationToken.IsCancellationRequested)
                    {
                        const int INTERVAL = 1000 / 30;
                        await Task.Delay(INTERVAL);

                        var events = ReadEvents();

                        if (events.Count > 0)
                        {
                            OnEvents(events);
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventFactory.FireEvent(EventType.OUTPUT_ERROR, $"Listener Exception:\n{ex}", serverDirectory);
                }
            }, cancelationToken);
        }

        public static List<Event> ReadEvents(DirectoryInfo serverDirectory, params EventType[] listenTypes)
        {
            List<Event> result = new List<Event>();

            var files = Directory.GetFiles(serverDirectory.FullName);

            foreach (string path in files)
            {
                string fileName = Path.GetFileName(path);

                if (fileName.Contains(EventFactory.SEPARATOR))
                {
                    string eventTypeKey = fileName.Split(EventFactory.SEPARATOR).Last();

                    if (Enum.TryParse(eventTypeKey, out EventType eventType))
                    {
                        if (listenTypes.Contains(eventType))
                        {
                            var @event = new Event
                            {
                                path = path,
                                eventType = eventType,
                                message = File.ReadAllText(path)
                            };

                            result.Add(@event);
                        }
                    }
                }
            }

            return result;
        }

        List<Event> ReadEvents()
        {
            return ReadEvents(serverDirectory, listenTypes);
        }
    }
}
