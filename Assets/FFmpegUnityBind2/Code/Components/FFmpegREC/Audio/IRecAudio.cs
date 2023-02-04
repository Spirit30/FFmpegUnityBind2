namespace FFmpegUnityBind2.Components
{
    interface IRecAudio
    {
        void StartRecording();
        void StopRecording(string savePath);
    }
}