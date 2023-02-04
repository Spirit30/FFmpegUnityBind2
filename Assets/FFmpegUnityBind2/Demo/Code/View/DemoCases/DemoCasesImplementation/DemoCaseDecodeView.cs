using System.IO;
using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseDecodeView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        ChooseDirectory chooseDirectoryView = null;

        [SerializeField]
        string frameBaneFormat = "frame_%04d.jpg";

        [SerializeField]
        string audioName = "audio.mp3";

        [SerializeField]
        int fps = 30;

        protected override void OnExecuteButton()
        {
            string framePathFormat = Path.Combine(chooseDirectoryView.InputPath, frameBaneFormat);
            string audioPath = Path.Combine(chooseDirectoryView.InputPath, audioName);
            var command = new VideoToImages(chooseFileView.InputPath, framePathFormat, audioPath, fps);
            ExecuteWithOutput(command);
        }

        protected override void Awake()
        {
            base.Awake();

            chooseFileView.OnChooseFileEvent += OnChooseFileEvent;
        }

        void OnChooseFileEvent(string path)
        {
            if (!chooseDirectoryView.IsDirectorySelected)
            {
                chooseDirectoryView.InputPath = Path.GetDirectoryName(path);
            }
        }
    }
}