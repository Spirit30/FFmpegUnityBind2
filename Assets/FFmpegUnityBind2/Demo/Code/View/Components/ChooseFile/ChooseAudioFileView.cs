using System.Collections.Generic;

namespace FFmpegUnityBind2.Demo
{
    class ChooseAudioFileView : ChooseFileView
    {
        static readonly List<string> commonAudioExtensions = new List<string>
        {
            ".mp3",".wav", ".aac", ".ogg", ".flac"
        };

        protected override List<string> Extensions => commonAudioExtensions;
    }
}