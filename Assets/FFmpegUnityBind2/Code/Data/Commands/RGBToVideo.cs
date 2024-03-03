using System.Linq;
using System.Text;
using UnityEngine;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class RGBToVideo : BaseCommand
    {
        readonly Vector2Int resolution;
        readonly float fps;
        readonly float totalTime;
        readonly RecAudioSource audioSource;
        readonly int crf;

        bool HasAudio => audioSource != RecAudioSource.None;

        public RGBToVideo(string framePath, string audioPath, string outputPath, Vector2Int resolution, float fps, float totalTime, RecAudioSource audioSource, int crf) : base(framePath, outputPath)
        {
            this.resolution = resolution;
            this.fps = fps;
            this.totalTime = totalTime;
            this.audioSource = audioSource;
            this.crf = crf;

            if (HasAudio)
            {
                InputPaths.Add(TryEnclosePath(audioPath));
            }
        }

        /// <summary>
        /// Example:
        /// Without audio:
        /// -y -f image2 -s 1280x720 -r 25 -pix_fmt rgb24 -vcodec rawvideo -i .../frame_%04d.rgb -vcodec libx264 -crf 23 -pix_fmt yuv420p -vf vflip .../output.mp4
        /// With audio:
        /// -y -f image2 -s 1280x720 -r 25 -pix_fmt rgb24 -vcodec rawvideo -i .../frame_%04d.rgb -i .../audio_recording.wav -ss 0 -t 30 -vcodec libx264 -crf 23 -pix_fmt yuv420p -vf vflip .../output.mp4
        /// File names:
        /// First frame file name example: .../frame_0000.rgb
        /// Second frame file name example: .../frame_0001.rgb
        /// ...
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            //Input Image sequence params
            stringBuilder
                .Append(REWRITE_INSTRUCTION)
                .Append(SPACE)
                .Append(FORCE_INPUT_OR_OUTPUT_INSTRUCTION)
                .Append(SPACE)
                .Append(IMAGE_FORMAT_INSTRUCTION)
                .Append(SPACE)
                .Append(RESOLUTION_INSTRUCTION)
                .Append(SPACE)
                .Append(resolution.x)
                .Append(X)
                .Append(resolution.y)
                .Append(SPACE)
                .Append(FPS_INSTRUCTION)
                .Append(SPACE)
                .Append(fps.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(SPACE)
                .Append(PIXEL_FORMAT_INSTRUCTION)
                .Append(SPACE)
                .Append(RGB_24_INSTRUCTION)
                .Append(SPACE)
                .Append(VIDEO_CODEC_INSTRUCTION)
                .Append(SPACE)
                .Append(RAW_VIDEO_INSTRUCTION)
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
                    .Append(totalTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
                .Append(VERTICAL_FLIP_INSTRUCTION)
                .Append(SPACE)
                .Append(VERTICAL_FLIP_ARG_INSTRUCTION)
                .Append(SPACE)
                .Append(OutputPath);

            return stringBuilder.ToString();
        }
    }
}
