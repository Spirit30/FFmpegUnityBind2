using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseCompressView : DemoCaseView
    {
        [SerializeField]
        ChooseFileView chooseFileView = null;

        [SerializeField]
        Slider qualitySlider = null;

        protected override void OnExecuteButton()
        {
            string directory = Path.GetDirectoryName(chooseFileView.InputPath);
            string fileName = Path.GetFileNameWithoutExtension(chooseFileView.InputPath);
            string outputPath = Path.Combine(directory, $"{fileName}Compressed.mp4");
            float crf = Mathf.Lerp(CRF.MIN_QUALITY, CRF.MAX_QUALITY, qualitySlider.value);
            var command = new CompressCommand(chooseFileView.InputPath, outputPath, crf);
            ExecuteWithOutput(command);
        }
    }
}
