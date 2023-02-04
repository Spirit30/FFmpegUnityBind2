using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseRECCameraView : DemoCaseRECBaseView
    {
        [SerializeField]
        CameraView cameraView = null;

        [SerializeField]
        TextureView textureView = null;

        protected override void OnStartCapturingButton(bool audio = true)
        {
            cameraView.Open();

            textureView.Open(cameraView.Texture);

            demoCaseSharedView.FFmpegREC.StartREC(RecAudioSource.Mic, Handlers);

            base.OnStartCapturingButton(false);
        }

        protected override void OnStopCapturingButton(bool audio = true)
        {
            demoCaseSharedView.FFmpegREC.StopREC();

            base.OnStopCapturingButton(audio);

            cameraView.Close();
            textureView.Close();
        }

        protected override void OnCancelButton()
        {
            demoCaseSharedView.FFmpegREC.Cancel();

            base.OnCancelButton();

            cameraView.Close();
            textureView.Close();
        }
    }
}