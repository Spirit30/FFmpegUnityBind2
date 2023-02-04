using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    abstract class DemoCaseRECBaseView : DemoCaseView
    {
        [SerializeField]
        protected Button stopButton = null;

        [SerializeField]
        protected Button cancelButton = null;

        float startTime;

        public override void OnSuccess(long executionId)
        {
            demoCaseSharedView.OnSuccess(executionId, demoCaseSharedView.FFmpegREC.Command.OutputPathOrigin);
        }

        protected override void OnExecuteButton()
        {
            OnStartCapturingButton();
        }

        protected override void Awake()
        {
            base.Awake();

            stopButton.onClick.AddListener(() => OnStopCapturingButton(true));
            cancelButton.onClick.AddListener(OnCancelButton);
        }

        protected virtual void OnStartCapturingButton(bool audio = true)
        {
            demoCaseSharedView.SetupSceneForStartCapturing(audio);

            startTime = Time.time;
        }

        protected virtual void OnStopCapturingButton(bool audio = true)
        {
            demoCaseSharedView.SetupSceneForStopCapturing(audio);
        }

        protected virtual void OnCancelButton()
        {
            demoCaseSharedView.SetupSceneForCancelCapturing();

            ClearView();
        }

        void Update()
        {
            executeButton.interactable = demoCaseSharedView.FFmpegREC.State == FFmpegRECState.Idle;

            if (stopButton.interactable = demoCaseSharedView.FFmpegREC.State == FFmpegRECState.Capturing)
            {
                Console.WriteLine($"Recording Time: {Time.time - startTime}");
            }
            else if (demoCaseSharedView.FFmpegREC.State == FFmpegRECState.Processing)
            {
                TrySetOrAddProcessView(demoCaseSharedView.FFmpegREC.FFmpegProcess);
            }

            if ((cancelButton.interactable = demoCaseSharedView.FFmpegREC.State > FFmpegRECState.Idle) && Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelButton();
            }
        }
    }
}
