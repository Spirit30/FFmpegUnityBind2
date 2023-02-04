using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    abstract class DemoCaseView : FFmpegCallbacksHandlerBase
    {
        [SerializeField]
        string demoCaseName = null;

        [SerializeField, TextArea]
        string demoCaseDescription = null;

        [SerializeField]
        protected DemoCaseSharedView demoCaseSharedView = null;

        [SerializeField]
        protected Button executeButton = null;

        [SerializeField]
        FFmpegProcessView ffmpegProcessViewOrigin = null;

        readonly ConsoleEventsHandler consoleHandler = new ConsoleEventsHandler();

        //<ExecutionId, Command>
        readonly Dictionary<long, BaseCommand> commands = new Dictionary<long, BaseCommand>();

        //<ExecutionId, Process>
        readonly Dictionary<long, FFmpegProcessView> processes = new Dictionary<long, FFmpegProcessView>();

        protected IFFmpegCallbacksHandler[] Handlers { get; private set; }

        #region FFMPEG CALLBACKS IMPLEMENTATION

        public override void OnStart(long executionId)
        {
            base.OnStart(executionId);

            Console.Clear();
        }

        public override void OnSuccess(long executionId)
        {
            base.OnSuccess(executionId);

            if (commands.ContainsKey(executionId))
            {
                var command = commands[executionId];

                demoCaseSharedView.OnSuccess(executionId, command.OutputPathOrigin);
            }
        }

        public override void OnCanceled(long executionId)
        {
            base.OnCanceled(executionId);

            Console.Clear();
            ClearView();
        }

        #endregion

        public void Open()
        {
            demoCaseSharedView.Open(demoCaseName, demoCaseDescription);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        protected virtual void Awake()
        {
            executeButton.onClick.AddListener(OnExecuteButton);

            Handlers = new IFFmpegCallbacksHandler[]
            {
                this, consoleHandler
            };
        }

        protected virtual void OnDisable()
        {
            ClearView();
        }

        protected FFmpegProcess ExecuteWithOutput(BaseCommand command)
        {
            var ffmpegProcess = ExecuteWithOutput(command.ToString());

            SetOrAddCommand(ffmpegProcess.Id, command);
            SetOrAddProcessView(ffmpegProcess);

            return ffmpegProcess;
        }

        protected FFmpegProcess ExecuteWithOutput(string command)
        {
            return FFmpeg.Execute(command, Handlers);
        }

        protected abstract void OnExecuteButton();

        protected void TrySetOrAddProcessView(FFmpegProcess ffmpegProcess)
        {
            if(!HasProcessView(ffmpegProcess))
            {
                SetOrAddProcessView(ffmpegProcess);
            }
        }

        protected void SetOrAddProcessView(FFmpegProcess ffmpegProcess)
        {
            SetOrAddProcessView(ffmpegProcessViewOrigin.Duplicate(ffmpegProcess));
        }

        protected void ClearView()
        {
            while (processes.Count > 0)
            {
                long firstKey = processes.First().Key;
                DestroyImmediate(processes[firstKey].gameObject);
                processes.Remove(firstKey);
            }
        }

        void SetOrAddCommand(long executionId, BaseCommand command)
        {
            if (commands.ContainsKey(executionId))
            {
                commands[executionId] = command;
            }
            else
            {
                commands.Add(executionId, command);
            }
        }

        void SetOrAddProcessView(FFmpegProcessView processView)
        {
            long executionId = processView.FFmpegProcess.Id;

            if (processes.ContainsKey(executionId))
            {
                DestroyImmediate(processes[executionId].gameObject);
                processes[executionId] = processView;
            }
            else
            {
                processes.Add(executionId, processView);
            }
        }

        bool HasProcessView(FFmpegProcess ffmpegProcess)
        {
            return processes.ContainsKey(ffmpegProcess.Id);
        }
    }
}