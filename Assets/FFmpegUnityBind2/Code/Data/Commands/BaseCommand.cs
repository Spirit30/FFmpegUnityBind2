using System;
using System.Collections.Generic;
using System.Linq;

namespace FFmpegUnityBind2
{
    using static Instructions;

    class BaseCommand
    {
        public List<string> InputPaths { get; }
        public string OutputPath { get; }
        public string OutputPathOrigin => TryRemoveEnclosingFromPath(OutputPath);

        public BaseCommand(string inputPath, string outputPath)
        {
            InputPaths = new List<string>
            {
                TryEnclosePath(inputPath)
            };
            OutputPath = TryEnclosePath(outputPath);
        }

        /// <summary>
        /// Example:
        /// -y -i .../input.mp4 .../output.mp3
        /// </summary>
        public override string ToString()
        {
            return $"{REWRITE_INSTRUCTION} {INPUT_INSTRUCTION} {InputPaths.First()} {OutputPath}";
        }

        /// <summary>
        /// Mechanism to avoid spaces problem in command path.
        /// </summary>
        protected string TryEnclosePath(string path)
        {
            ValidatePath(path);

#if UNITY_EDITOR || UNITY_STANDALONE
            if (path[0] != DOUBLE_QUOTE)
            {
                return $"{DOUBLE_QUOTE}{path}{DOUBLE_QUOTE}";
            }
#endif
            return path;
        }

        protected string TryRemoveEnclosingFromPath(string path)
        {
            return path.Replace(DOUBLE_QUOTE.ToString(), string.Empty);
        }

        void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new Exception($"Empty path.");
            }
        }
    }
}