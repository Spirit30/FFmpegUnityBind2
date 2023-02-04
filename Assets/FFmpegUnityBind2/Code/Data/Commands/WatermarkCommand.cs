#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
#define DESKTOP
#elif UNITY_ANDROID || UNITY_IOS
#define MOBILE
#endif
using System.Linq;
using System.Text;
using UnityEngine;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class WatermarkCommand : BaseCommand
    {
        readonly float imageScale;
        readonly Vector2 imagePosition;

        public WatermarkCommand(string inputPath, string imagePath, string outputPath, float imageScale, Vector2 imagePosition) : base(inputPath, outputPath)
        {
            this.imageScale = imageScale;
            this.imagePosition = imagePosition;

            InputPaths.Add(TryEnclosePath(imagePath));
        }

        /// <summary>
        /// -y -i .../watermark.png -i .../input.mp4 -filter_complex \
        /// "[0:v]scale=iw*0.25:ih*0.25 [ovrl], [1:v][ovrl]overlay=x=(main_w-overlay_w)*0.95:y=(main_h-overlay_h)*0.05" \
        /// .../output.mp4
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .Append(REWRITE_INSTRUCTION)
                .Append(SPACE)
                .Append(INPUT_INSTRUCTION)
                .Append(SPACE)
                .Append(InputPaths.Last())
                .Append(SPACE)
                .Append(INPUT_INSTRUCTION)
                .Append(SPACE)
                .Append(InputPaths.First())
                .Append(SPACE)
                .Append(FILTER_COMPLEX_INSTRUCTION)
                .Append(SPACE);

            var filterStringBuilder = new StringBuilder();
#if DESKTOP
            filterStringBuilder.Append(DOUBLE_QUOTE);
#endif
            filterStringBuilder.
#if MOBILE
                  Append('\'').
#endif
                  Append("[0:v]scale=iw*").
                  Append(imageScale).
                  Append(":ih*").
                  Append(imageScale).
                  Append(" [ovrl], [1:v][ovrl]overlay=x=(main_w-overlay_w)*").
                  Append(imagePosition.x).
                  Append(":y=(main_h-overlay_h)*").
#if MOBILE
                  Append(imagePosition.y).
                  Append('\'');
#else
                  Append(imagePosition.y);
#endif

#if DESKTOP
            filterStringBuilder.Append(DOUBLE_QUOTE);
#endif

            stringBuilder
                .Append(filterStringBuilder)
                .Append(SPACE)
                .Append(OutputPath);

            return stringBuilder.ToString();
        }
    }
}
