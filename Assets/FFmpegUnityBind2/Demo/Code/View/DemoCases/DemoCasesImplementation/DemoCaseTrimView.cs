using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseTrimView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        InputField startTimeInput = null;

        [SerializeField]
        InputField durationInput = null;

        protected override void OnExecuteButton()
        {
            string directory = Path.GetDirectoryName(chooseFileView.InputPath);
            string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
            string outputPath = Path.Combine(directory, $"{fileName}Trimmed.mp4");
            float startTime = float.Parse(startTimeInput.text);
            float duration = float.Parse(durationInput.text);
            var command = new TrimCommand(chooseFileView.InputPath, outputPath, startTime, duration);
            ExecuteWithOutput(command);
        }
    }
}