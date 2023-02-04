using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseAppendFullView : DemoCaseView
    {
        [SerializeField]
        ChooseDirectory elementsDirectory = null;

        [SerializeField]
        InputField extensionInput = null;

        protected override void OnExecuteButton()
        {
            var files = Directory.GetFiles(elementsDirectory.InputPath);
            var videoElements = files.Where(f => ChooseVideoFileView.IsVideo(f)).ToArray();

            if (videoElements.Length > 1)
            {
                string outputPath = Path.Combine(elementsDirectory.InputPath, "AppendFullResult" + extensionInput.text);
                var command = new AppendFullCommand(videoElements, outputPath);
                ExecuteWithOutput(command);
            }
            else
            {
                Debug.LogError("Selected directory should have at least 2 video files.");
            }
        }
    }
}
