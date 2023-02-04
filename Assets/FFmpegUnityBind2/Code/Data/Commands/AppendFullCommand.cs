using System.Text;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class AppendFullCommand : BaseCommand
    {
        readonly string[] videoElelements;

        public AppendFullCommand(string[] videoElelements, string outputPath) : base(string.Empty, outputPath)
        {
            this.videoElelements = videoElelements;
        }

        /// <summary>
        /// -y -i .../input1.mkv - i .../input2.webm \
        /// -filter_complex "[0:v:0] [0:a:0] [1:v:0] [1:a:0] concat=n=2:v=1:a=1 [v] [a]" \
        /// -map "[v]" - map "[a]" <encoding_options> .../output.mp4
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .Append(REWRITE_INSTRUCTION)
                .Append(SPACE);

            var filterStringBuilder = new StringBuilder();
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
            filterStringBuilder.Append(DOUBLE_QUOTE);
#endif
            for (int i = 0; i < videoElelements.Length; ++i)
            {
                stringBuilder
                    .Append(INPUT_INSTRUCTION)
                    .Append(SPACE)
                    .Append(TryEnclosePath(videoElelements[i]))
                    .Append(SPACE);

                filterStringBuilder
                    .Append(string.Format(VIDEO_COMPLEX_FORMAT_INSTRUCTION, i))
                    .Append(string.Format(AUDIO_COMPLEX_FORMAT_INSTRUCTION, i));
            }

            filterStringBuilder.
                  Append(string.Format(CONCAT_COMPLEX_FORMAT_INSTRUCTION, CONCAT_INSTRUCTION, videoElelements.Length))
                  .Append(SPACE)
                  .Append(VIDEO_INSTRUCTION)
                  .Append(SPACE)
                  .Append(AUDIO_INSTRUCTION);
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
            filterStringBuilder.Append(DOUBLE_QUOTE);
#endif

            stringBuilder
                .Append(FILTER_COMPLEX_INSTRUCTION)
                .Append(SPACE)
                .Append(filterStringBuilder.ToString())
                .Append(SPACE)
                .Append(MAP_INSTRUCTION)
                .Append(SPACE)
                .Append(VIDEO_INSTRUCTION)
                .Append(SPACE)
                .Append(MAP_INSTRUCTION)
                .Append(SPACE)
                .Append(AUDIO_INSTRUCTION)
                .Append(SPACE)
                .Append(PRESET_INSTRUCTION)
                .Append(SPACE)
                .Append(ULTRASAFE_INSTRUCTION)
                .Append(SPACE)
                .Append(OutputPath);

            return stringBuilder.ToString();
        }
    }
}
