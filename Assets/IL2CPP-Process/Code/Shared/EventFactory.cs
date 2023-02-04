using System.IO;

namespace IL2CPP.Process.Shared
{
    public static class EventFactory
    {
        public const char SEPARATOR = '-';
        static long eventId;
        static long NextEventId => eventId++;

        public static void FireEvent(EventType type, string message, DirectoryInfo serverDirectory)
        {
            long eventId = NextEventId;
            string tempEventName = BuildTempEventName(eventId);
            string eventName = BuildEventName(type, eventId);
            string tempEventPath = Path.Combine(serverDirectory.FullName, tempEventName);
            string eventPath = Path.Combine(serverDirectory.FullName, eventName);
            File.WriteAllText(tempEventPath, message);
            File.Move(tempEventPath, eventPath);
        }

        static string BuildTempEventName(long eventId)
        {
            return $"{eventId}{SEPARATOR}TEMP";
        }

        static string BuildEventName(EventType type, long eventId)
        {
            return $"{eventId}{SEPARATOR}{type}";
        }
    }
}
