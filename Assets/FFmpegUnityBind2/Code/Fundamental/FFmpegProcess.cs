using FFmpegUnityBind2.Internal;

namespace FFmpegUnityBind2
{
    public class FFmpegProcess
    {
        public long Id { get; }

        public float Progress => processInternal.Progress;

        public float ProgressPercent => processInternal.Progress * 100;

        public bool IsDone => Progress >= 1;

        readonly FFmpegProgress processInternal;

        internal FFmpegProcess(long id, FFmpegProgress processInternal)
        {
            Id = id;
            this.processInternal = processInternal;
        }

        public void Cancel()
        {
            FFmpeg.Cancel(Id);
        }
    }
}