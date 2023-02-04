using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class VideoToImages : BaseCommand
    {
        readonly string audioPath;
        readonly int fps;

        public VideoToImages(string inputPath, string framePathFormat, string audioPath, int fps) : base(inputPath, framePathFormat)
        {
            this.audioPath = TryEnclosePath(audioPath);
            this.fps = fps;
        }

        /// <summary>
        /// Example:
        /// -y -i .../input.mp4 -r 30 .../frame_%04d.jpg .../audio.mp3 
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} {FPS_INSTRUCTION} {fps} " +
                $"{OutputPath} {audioPath}";
        }
    }
}
