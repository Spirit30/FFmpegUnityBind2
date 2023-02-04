using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FFmpegUnityBind2.Components
{
    class RecSystemAudio : MonoBehaviour, IRecAudio
    {
        readonly List<float> audioData = new List<float>();
        float startTime;
        int channelsCount;
        bool isRecording;

        public void StartRecording()
        {
            isRecording = true;
            startTime = Time.time;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (isRecording)
            {
                audioData.AddRange(data);
                channelsCount = channels;
            }
        }

        public void StopRecording(string savePath)
        {
            isRecording = false;
            int durationInSec = Mathf.CeilToInt(Time.time - startTime);

            //Create file
            AudioClip buffer =
                AudioClip.Create(
                    Path.GetFileNameWithoutExtension(savePath),
                    AudioSettings.outputSampleRate * channelsCount * durationInSec,
                    channelsCount,
                    AudioSettings.outputSampleRate,
                    false);

            buffer.SetData(audioData.ToArray(), 0);

            WAV.Save(savePath, buffer);

            audioData.Clear();
        }
    }
}