using System.IO;
using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class AddSoundFastCommand : BaseCommand
    {
        public AddSoundFastCommand(string inputPath, string audioPath, string outputPath) : base(inputPath, outputPath)
        {
            if (File.Exists(audioPath))
            {
                InputPaths.Add(TryEnclosePath(audioPath));
            }
        }

        /// <summary>
        /// Example:
        /// -y -i .../input.mp4 -i .../audio.aac -c copy -map 0:v -map 1:a -shortest .../output.mp4
        /// aac is compatible with mp4 - no need full re-encoding.
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{INPUT_INSTRUCTION} {InputPaths.Last()} " +
                $"{C_CODEC_INSTRUCTION} {COPY_INSTRUCTION} {MAP_INSTRUCTION} {FIRST_INPUT_VIDEO_CHANNEL_INSTRUCTION} " +
                $"{MAP_INSTRUCTION} {SECOND_INPUT_AUDIO_CHANNEL_INSTRUCTION} {SHORTEST_INSTRUCTION} " +
                $"{OutputPath}";
        }
    }
}
