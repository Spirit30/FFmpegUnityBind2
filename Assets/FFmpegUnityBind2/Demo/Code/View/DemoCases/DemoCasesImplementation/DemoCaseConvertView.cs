using FFmpegUnityBind2.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseConvertView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        InputField extensionInput = null;

        protected override void Awake()
        {
            base.Awake();

            extensionInput.SetTextWithoutNotify(PlatformVideoExtension.GetCurrent());
        }

        protected override void OnExecuteButton()
        {
            string directory = Path.GetDirectoryName(chooseFileView.InputPath);
            string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
            string outputPath = Path.Combine(directory, $"{fileName}Converted{extensionInput.text}");
            var command = new BaseCommand(chooseFileView.InputPath, outputPath);
            ExecuteWithOutput(command);
        }
    }
}