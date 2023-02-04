using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseAppendFastView : DemoCaseView
    {
        [SerializeField]
        ChooseDirectory elementsDirectory = null;

        [SerializeField]
        InputField extensionInput = null;

        protected override void OnExecuteButton()
        {
            var videoElements = Directory.GetFiles(elementsDirectory.InputPath, $"*{extensionInput.text}");

            if (videoElements.Length > 1)
            {
                string outputPath = Path.Combine(elementsDirectory.InputPath, "AppendFastResult" + extensionInput.text);
                var command = new AppendFastCommand(videoElements, outputPath);
                ExecuteWithOutput(command);
            }
            else
            {
                Debug.LogError($"Selected directory should have at least 2 files of \"{extensionInput.text}\" extension.");
            }
        }
    }
}
