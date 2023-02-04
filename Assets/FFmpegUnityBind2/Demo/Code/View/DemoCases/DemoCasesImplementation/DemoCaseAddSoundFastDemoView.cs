using System.IO;
using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseAddSoundFastDemoView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        ChooseFileView chooseAudioFileView = null;

        protected override void OnExecuteButton()
        {
            string directory = Path.GetDirectoryName(chooseFileView.InputPath);
            string extension = Path.GetExtension(chooseFileView.InputPath);
            string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
            string outputPath = Path.Combine(directory, $"{fileName}PlusAudio{extension}");
            var command = new AddSoundFastCommand(chooseFileView.InputPath, chooseAudioFileView.InputPath, outputPath);
            ExecuteWithOutput(command);
        }
    }
}
