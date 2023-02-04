namespace FFmpegUnityBind2
{
    public interface IFFmpegCallbacksHandler
    {
        void OnStart(long executionId);
        void OnLog(long executionId, string message);
        void OnWarning(long executionId, string message);
        void OnError(long executionId, string message);
        void OnSuccess(long executionId);
        void OnCanceled(long executionId);
        void OnFail(long executionId);
    }
}