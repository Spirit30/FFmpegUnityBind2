namespace FFmpegUnityBind2.Demo
{
    class DemoCaseRECView : DemoCaseRECBaseView
    {
        protected override void OnStartCapturingButton(bool audio = true)
        {
            demoCaseSharedView.FFmpegREC.StartREC(RecAudioSource.System, Handlers);

            base.OnStartCapturingButton(audio);
        }

        protected override void OnStopCapturingButton(bool audio = true)
        {
            demoCaseSharedView.FFmpegREC.StopREC();

            base.OnStopCapturingButton(audio);
        }

        protected override void OnCancelButton()
        {
            demoCaseSharedView.FFmpegREC.Cancel();

            base.OnCancelButton();
        }
    }
}