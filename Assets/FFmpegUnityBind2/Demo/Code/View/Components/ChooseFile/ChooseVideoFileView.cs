using System.Collections.Generic;
using System.IO;

namespace FFmpegUnityBind2.Demo
{
    class ChooseVideoFileView : ChooseFileView
    {
        static readonly List<string> commonVideoExtensions = new List<string>
        {
            ".mp4", ".avi", ".wmv", ".m4a", ".mov", ".3gp", ".mkv", ".webm"
        };

        protected override List<string> Extensions => commonVideoExtensions;

        public static bool IsVideo(string path)
        {
            string extension = Path.GetExtension(path);
            return commonVideoExtensions.Contains(extension);
        }
    }
}