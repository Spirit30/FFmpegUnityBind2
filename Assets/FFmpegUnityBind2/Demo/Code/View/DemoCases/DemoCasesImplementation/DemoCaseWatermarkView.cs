using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseWatermarkView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        ChooseFileView chooseImageFileView = null;

        [SerializeField]
        InputField imageScaleInput = null;

        [SerializeField]
        InputField imageXPositionInput = null;

        [SerializeField]
        InputField imageYPositionInput = null;

        protected override void OnExecuteButton()
        {
            string directory = Path.GetDirectoryName(chooseFileView.InputPath);
            string extension = Path.GetExtension(chooseFileView.InputPath);
            string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
            string outputPath = Path.Combine(directory, $"{fileName}PlusWatermark{extension}");
            float imageScale = float.Parse(imageScaleInput.text);
            var imagePosition = new Vector2(float.Parse(imageXPositionInput.text), float.Parse(imageYPositionInput.text));
            var command = new WatermarkCommand(chooseFileView.InputPath, chooseImageFileView.InputPath, outputPath, imageScale, imagePosition);
            ExecuteWithOutput(command);
        }
    }
}
