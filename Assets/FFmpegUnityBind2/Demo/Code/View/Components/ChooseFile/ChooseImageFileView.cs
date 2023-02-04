using System.Collections.Generic;

namespace FFmpegUnityBind2.Demo
{
    class ChooseImageFileView : ChooseFileView
    {
        static readonly List<string> commonImageExtensions = new List<string>
        {
            ".png",".jpg"
        };

        protected override List<string> Extensions => commonImageExtensions;
    }
}