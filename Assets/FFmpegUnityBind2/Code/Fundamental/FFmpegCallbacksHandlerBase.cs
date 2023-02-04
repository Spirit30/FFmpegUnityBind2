using System;
using UnityEngine;

namespace FFmpegUnityBind2
{
    using Internal;

    class FFmpegCallbacksHandlerBase : MonoBehaviour, IFFmpegCallbacksHandler
    {
        public event Action<long, FFmpegCallbacksHandlerBase> OnStartEvent = delegate { };
        public event Action<long, string, FFmpegCallbacksHandlerBase> OnLogEvent = delegate { };
        public event Action<long, string, FFmpegCallbacksHandlerBase> OnWarningEvent = delegate { };
        public event Action<long, string, FFmpegCallbacksHandlerBase> OnErrorEvent = delegate { };
        public event Action<long, FFmpegCallbacksHandlerBase> OnSuccessEvent = delegate { };
        public event Action<long, FFmpegCallbacksHandlerBase> OnFailEvent = delegate { };
        public event Action<long, FFmpegCallbacksHandlerBase> OnCanceledEvent = delegate { };
        public event Action<long, FFmpegCallbacksHandlerBase> OnFinishEvent = delegate { };

        public EventType EventState { get; private set; }
        public bool IsProcessing { get; private set; }

        public virtual void OnStart(long executionId)
        {
            EventState = EventType.OnStart;
            IsProcessing = true;
            OnStartEvent(executionId, this);
        }

        public virtual void OnLog(long executionId, string message)
        {
            EventState = EventType.OnLog;
            OnLogEvent(executionId, message, this);
        }

        public virtual void OnWarning(long executionId, string message)
        {
            EventState = EventType.OnWarning;
            OnWarningEvent(executionId, message, this);
        }

        public virtual void OnError(long executionId, string message)
        {
            EventState = EventType.OnError;
            OnErrorEvent(executionId, message, this);
        }

        public virtual void OnSuccess(long executionId)
        {
            EventState = EventType.OnSuccess;
            OnSuccessEvent(executionId, this);
            OnFinish(executionId);
        }

        public virtual void OnFail(long executionId)
        {
            EventState = EventType.OnFail;
            OnFailEvent(executionId, this);
            OnFinish(executionId);
        }

        public virtual void OnCanceled(long executionId)
        {
            EventState = EventType.OnCanceled;
            OnCanceledEvent(executionId, this);
            OnFinish(executionId);
        }

        public virtual void OnFinish(long executionId)
        {
            IsProcessing = false;
            OnFinishEvent(executionId, this);
        }
    }
}
