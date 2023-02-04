using System;
using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class TrimCommand : BaseCommand
    {
        readonly float startTime;
        readonly float durationSec;

        public TrimCommand(string inputPath, string outputPath, float startTime, float durationSec) : base(inputPath, outputPath)
        {
            this.startTime = startTime;
            this.durationSec = durationSec;
        }

        /// <summary>
        /// Example:
        /// -y -i .../input.mp4 -ss 00:00:05 -t 00:00:05 -vcodec libx264 .../output.mp4
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{ACCURATE_SEEK_INSTRUCTION} {TimeSpan.FromSeconds(startTime)} {TIME_INSTRUCTION} {durationSec} " +
                $"{VIDEO_CODEC_INSTRUCTION} {LIB_X264_INSTRUCTION} {OutputPath}";
        }
    }
}
