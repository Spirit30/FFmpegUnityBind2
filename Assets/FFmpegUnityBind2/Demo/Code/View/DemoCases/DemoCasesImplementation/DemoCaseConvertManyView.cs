using FFmpegUnityBind2.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseConvertManyView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        InputField extensionInput = null;

        [SerializeField]
        InputField copiesCount = null;

        protected override void Awake()
        {
            base.Awake();

            extensionInput.SetTextWithoutNotify(PlatformVideoExtension.GetCurrent());
        }

        protected override void OnExecuteButton()
        {
            for (int n = 1; n <= int.Parse(copiesCount.text); ++n)
            {
                string directory = Path.GetDirectoryName(chooseFileView.InputPath);
                string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
                string outputPath = Path.Combine(directory, $"{fileName}Converted{n}{extensionInput.text}");
                var command = new BaseCommand(chooseFileView.InputPath, outputPath);
                ExecuteWithOutput(command);
            }
        }
    }
}