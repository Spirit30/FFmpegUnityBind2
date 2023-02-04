namespace FFmpegUnityBind2.Internal
{
    class FFmpegProgress : IFFmpegCallbacksHandler
    {
        float durationMS;
        float progress;

        public float Progress => progress;

        public void OnStart(long executionId)
        {
        }

        public void OnLog(long executionId, string message)
        {
            FFmpegProgressParser.Parse(message, ref durationMS, ref progress);
        }

        public void OnWarning(long executionId, string message)
        {
        }

        public void OnError(long executionId, string message)
        {
        }

        public void OnSuccess(long executionId)
        {
            FinishProgress();
        }

        public void OnCanceled(long executionId)
        {
            FinishProgress();
        }

        public void OnFail(long executionId)
        {
            FinishProgress();
        }

        void FinishProgress()
        {
            progress = 1;
        }
    }
}