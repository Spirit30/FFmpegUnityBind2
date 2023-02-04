using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class CompressCommand : BaseCommand
    {
        readonly float crf;

        public CompressCommand(string inputPath, string outputPath, float crf) : base(inputPath, outputPath)
        {
            this.crf = crf;
        }

        /// <summary>
        /// -y -i .../input.mp4 -c:v libx264 -crf 23 .../output.mp4
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{CODEC_VIDEO_INSTRUCTION} {LIB_X264_INSTRUCTION} {CONSTANT_RATE_FACTOR_INSTRUCTION} {crf} " +
                $"{OutputPath}";
        }
    }
}
