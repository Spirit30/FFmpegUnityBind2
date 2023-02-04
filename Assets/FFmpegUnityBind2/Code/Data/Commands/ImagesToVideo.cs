using System.IO;
using System.Linq;
using System.Text;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class ImagesToVideo : BaseCommand
    {
        readonly float fps;
        readonly float totalTime;
        readonly int crf;

        bool HasAudio => InputPaths.Count > 1 && File.Exists(InputPaths.Last());

        public ImagesToVideo(string framePathFormat, string audioPath, string outputPath, float fps, float totalTime, int crf) : base(framePathFormat, outputPath)
        {
            this.fps = fps;
            this.totalTime = totalTime;
            this.crf = crf;

            if (File.Exists(audioPath))
            {
                InputPaths.Add(TryEnclosePath(audioPath));
            }
        }

        /// <summary>
        /// Example:
        /// Without audio:
        /// -y -framerate 25 -f image2 -i .../frame_%04d.jpg -vcodec libx264 -crf 23 -pix_fmt yuv420p .../output.mp4
        /// With audio:
        /// -y -framerate 25 -f image2 -i .../frame_%04d.jpg -i .../audio_recording.wav -ss 0 -t -vcodec libx264 -crf 23 -pix_fmt yuv420p .../output.mp4
        /// File names:
        /// First frame file name example: .../frame_0000.jpg
        /// Second frame file name example: .../frame_0001.jpg
        /// ...
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            //Input Image sequence params
            stringBuilder
                .Append(REWRITE_INSTRUCTION)
                .Append(SPACE)
                .Append(FRAMERATE_INSTRUCTION)
                .Append(SPACE)
                .Append(fps)
                .Append(SPACE)
                .Append(FORCE_INPUT_OR_OUTPUT_INSTRUCTION)
                .Append(SPACE)
                .Append(IMAGE_FORMAT_INSTRUCTION)
                .Append(SPACE)
                .Append(INPUT_INSTRUCTION)
                .Append(SPACE)
                .Append(InputPaths.First());

            //Input Audio params
            if (HasAudio)
            {
                stringBuilder
                    .Append(SPACE)
                    .Append(INPUT_INSTRUCTION)
                    .Append(SPACE)
                    .Append(InputPaths.Last())
                    .Append(SPACE)
                    .Append(ACCURATE_SEEK_INSTRUCTION)
                    .Append(SPACE)
                    .Append(ZERO_INSTRUCTION)
                    .Append(SPACE)
                    .Append(TIME_INSTRUCTION)
                    .Append(SPACE)
                    .Append(totalTime);
            }

            //Output Video params
            stringBuilder
                .Append(SPACE)
                .Append(VIDEO_CODEC_INSTRUCTION)
                .Append(SPACE)
                .Append(LIB_X264_INSTRUCTION)
                .Append(SPACE)
                .Append(CONSTANT_RATE_FACTOR_INSTRUCTION)
                .Append(SPACE)
                .Append(crf)
                .Append(SPACE)
                .Append(PIXEL_FORMAT_INSTRUCTION)
                .Append(SPACE)
                .Append(YUV_420P_INSTRUCTION)
                .Append(SPACE)
                .Append(OutputPath);

            return stringBuilder.ToString();
        }
    }
}
