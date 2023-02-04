using System.IO;
using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class AddSoundFullCommand : BaseCommand
    {
        public AddSoundFullCommand(string inputPath, string audioPath, string outputPath) : base(inputPath, outputPath)
        {
            if (File.Exists(audioPath))
            {
                InputPaths.Add(TryEnclosePath(audioPath));
            }
        }

        /// <summary>
        /// Example:
        /// -y -i .../input.mp4 -i .../audio.wav .../output.mp4
        /// .wav is not compatible with mp4. Full re-encoding easily handles this.
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{INPUT_INSTRUCTION} {InputPaths.Last()} {OutputPath}";
        }
    }
}
