using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class YouTubeStreamCommand : BaseCommand
    {
        const string YOUTUBE_STREAM_URL = "rtmp://a.rtmp.youtube.com/live2/";

        public YouTubeStreamCommand(string inputPath, string streamKey) : base(inputPath, YOUTUBE_STREAM_URL + streamKey)
        {
        }

        /// <summary>
        /// Example:
        /// -re -i .../input.mp4 -f flv rtmp://a.rtmp.youtube.com/live2/xxxx-xxxx-xxxx-xxxx-xxxx
        /// </summary>
        public override string ToString()
        {
            return $"{NATIVE_FRAMERATE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{FORCE_INPUT_OR_OUTPUT_INSTRUCTION} {FLV_INSTRUCTION} {OutputPath}";
        }
    }
}
