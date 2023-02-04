using System.IO;
using System.Linq;
using System.Text;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class AppendFastCommand : BaseCommand
    {
        public AppendFastCommand(string[] videoElelements, string outputPath) : base(
            //Issue 1:
            //https://superuser.com/a/943258
            Path.ChangeExtension(outputPath.Replace(@"\", "/"), ".txt"), 
            outputPath)
        {
            WritePathsToFile(videoElelements);
        }

        void WritePathsToFile(string[] inputPaths)
        {
            var stringBuilder = new StringBuilder("# File with input videos\n");

            foreach (string inputPath in inputPaths)
            {
                stringBuilder
                    .Append($"file '")
                    //Issue 2:
                    //https://superuser.com/a/943258
                    .Append(Path.GetFileName(inputPath))
                    .Append("'\n");
            }

            string inputPathsFilePath = TryRemoveEnclosingFromPath(InputPaths.First());

            using (FileStream fileStream = File.Create(inputPathsFilePath))
            {
                byte[] buffer = new UTF8Encoding(true).GetBytes(stringBuilder.ToString());
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Example:
        /// -y -f concat -safe 0 -i .../inputPathsFile.txt -c copy .../output.mp4 
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {FORCE_INPUT_OR_OUTPUT_INSTRUCTION} {CONCAT_INSTRUCTION} " +
                $"{SAFE_INSTRUCTION} {ZERO_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} " +
                $"{C_CODEC_INSTRUCTION} {COPY_INSTRUCTION} {OutputPath}";
        }
    }
}
