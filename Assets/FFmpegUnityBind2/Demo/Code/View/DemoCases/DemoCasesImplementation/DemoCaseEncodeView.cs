using System.IO;
using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseEncodeView : DemoCaseView
    {
        [SerializeField]
        ChooseDirectory chooseInputDirectoryView = null;

        [SerializeField]
        ChooseDirectory chooseOutputDirectoryView = null;

        [SerializeField]
        string frameBaneFormat = "frame_%04d.jpg";

        [SerializeField]
        string audioName = "audio.mp3";

        [SerializeField]
        string videoName = "video.mp4";

        [SerializeField]
        int fps = 30;

        protected override void OnExecuteButton()
        {
            string framePathFormat = Path.Combine(chooseInputDirectoryView.InputPath, frameBaneFormat);
            string audioPath = Path.Combine(chooseInputDirectoryView.InputPath, audioName);
            string outputPath = Path.Combine(chooseOutputDirectoryView.InputPath, videoName);
            int framesCount = Directory.GetFiles(chooseInputDirectoryView.InputPath, $"*{Path.GetExtension(frameBaneFormat)}").Length;
            float totalTime = 1.0f / fps * framesCount;
            var command = new ImagesToVideo(framePathFormat, audioPath, outputPath, fps, totalTime, CRF.DEFAULT_QUALITY);
            ExecuteWithOutput(command);
        }

        protected override void Awake()
        {
            base.Awake();

            chooseInputDirectoryView.OnChooseDirectoryEvent += OnChooseFileEvent;
        }

        void OnChooseFileEvent(string path)
        {
            if (!chooseOutputDirectoryView.IsDirectorySelected)
            {
                chooseOutputDirectoryView.InputPath = path;
            }
        }
    }
}
