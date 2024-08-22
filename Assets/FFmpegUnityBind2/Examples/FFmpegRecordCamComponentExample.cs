using System.Collections.Generic;
using FFmpegUnityBind2.Components;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Example
{
    internal class FFmpegRecordCamComponentExample : FFmpegCallbacksHandlerBase
    {
        [SerializeField]
        private FFmpegREC ffmpegREC = null;

        [SerializeField]
        protected Button startButton = null;

        [SerializeField]
        protected Button stopButton = null;

        [SerializeField]
        protected Button cancelButton = null;

        //<ExecutionId, Command>
        readonly Dictionary<long, BaseCommand> commands = new Dictionary<long, BaseCommand>();

        protected IFFmpegCallbacksHandler[] Handlers { get; private set; }

        //<ExecutionId, Process>
        //readonly Dictionary<long, FFmpegProcessView> processes = new Dictionary<long, FFmpegProcessView>();
        float startTime;

        public override void OnSuccess(long executionId)
        {
            //demoCaseSharedView.OnSuccess(executionId, demoCaseSharedView.FFmpegREC.Command.OutputPathOrigin);
            base.OnSuccess(executionId);
        }

        private void Awake()
        {
            startButton.onClick.AddListener(() => OnStartCapturingButton(true));
            stopButton.onClick.AddListener(() => OnStopCapturingButton(true));
            cancelButton.onClick.AddListener(OnCancelButton);
        }

        private void OnStartCapturingButton(bool audio = true)
        {
            ffmpegREC.StartREC(RecAudioSource.System, Handlers);
            startTime = Time.time;
        }

        private void OnStopCapturingButton(bool audio = true)
        {
            ffmpegREC.StopREC();
        }

        private void OnCancelButton()
        {
            ffmpegREC.Cancel();
        }

        void Update()
        {
            startButton.interactable = ffmpegREC.State == FFmpegRECState.Idle;

            if (stopButton.interactable = ffmpegREC.State == FFmpegRECState.Capturing)
            {
                print($"Recording Time: {Time.time - startTime}");
            }
            else if (ffmpegREC.State == FFmpegRECState.Processing)
            {
                //TrySetOrAddProcessView(FFmpegREC.FFmpegProcess);
            }

            if ((cancelButton.interactable = ffmpegREC.State > FFmpegRECState.Idle) && Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelButton();
            }
        }

    }
}