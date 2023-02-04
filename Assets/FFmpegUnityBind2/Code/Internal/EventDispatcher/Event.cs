using System;

namespace FFmpegUnityBind2.Internal
{
    class Event
    {
        readonly IFFmpegCallbacksHandler handler;
        readonly long executionId;
        readonly EventType type;
        readonly string message;

        public Event(IFFmpegCallbacksHandler handler, long executionId, EventType type, string message = null)
        {
            this.handler = handler;
            this.executionId = executionId;
            this.type = type;
            this.message = message;
        }

        public void Handle()
        {
            switch (type)
            {
                case EventType.OnStart:

                    handler.OnStart(executionId);
                    break;

                case EventType.OnLog:

                    handler.OnLog(executionId, message);
                    break;

                case EventType.OnWarning:

                    handler.OnWarning(executionId, message);
                    break;

                case EventType.OnError:

                    handler.OnError(executionId, message);
                    break;

                case EventType.OnSuccess:

                    handler.OnSuccess(executionId);
                    break;

                case EventType.OnCanceled:

                    handler.OnCanceled(executionId);
                    break;

                case EventType.OnFail:

                    handler.OnFail(executionId);
                    break;

                default:

                    throw new ArgumentException($"Invalid Event Type: {type}");
            }
        }

        public override string ToString()
        {
            return $"{type} Id: {executionId} {message}";
        }
    }
}
