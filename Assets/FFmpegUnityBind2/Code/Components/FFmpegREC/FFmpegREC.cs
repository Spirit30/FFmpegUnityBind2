using FFmpegUnityBind2.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FFmpegUnityBind2.Components
{
    /// <summary>
    /// Record Video from Camera component system.
    /// </summary>
    [RequireComponent(typeof(Camera), typeof(RecMicAudio), typeof(RecSystemAudio))]
    class FFmpegREC : FFmpegCallbacksHandlerBase
    {
        [SerializeField, Header("Set 0 for max resolution. Can affect performance: the higher -> the slower.")]
        Vector2Int resolution = new Vector2Int(854, 480);

        [SerializeField, Header("Change before initialization.")]
        RecAudioSource audioSource = RecAudioSource.System;

        [SerializeField, Header("Targeted FPS. Can affect performance: the higher -> the slower.")]
        int targetFPS = 30;

        [SerializeField, Range(CRF.MIN_QUALITY, CRF.MAX_QUALITY), Header("Constant Rate Factor (CRF). 0 is lossless, 23 is the default, and 51 is worst.")]
        int quality = CRF.DEFAULT_QUALITY;

        [SerializeField, Range(1, 8), Header("Needed to free RAM from frames. The more RAM you expect -> the less working threads you can have.")]
        int writingThreadsCount = 8;

        [SerializeField, Header("Clear Flags used during Recording.")]
        CameraClearFlags clearFlags = CameraClearFlags.SolidColor;

        [SerializeField, Header("Color Background used during Recording if flags are in \"Solid Color\".")]
        Color backgroundColor = Color.green;

        const string CACHE_DIRECTORY_NAME = "RecordingCache";
        const string FILE_FORMAT = "frame_{0}.rgb";
        const string NUMBER_FORMAT = "0000";
        const string FFMPEG_NUMBER_FORMAT = "%04d";
        const string AUDIO_EXTENSION = ".wav";
        const string VIDEO_NAME = "REC.mp4";
        string cacheDir;
        string imgeFilePathFormat;
        string audioPath;
        string outputVideoPath;

        CameraClearFlags initialClearFlags;
        Color initialBackgroundColor;
        Vector2Int initialResultion;
        Rect camRect;
        public Camera Camera { get; private set; }

        float startTime;
        float frameInterval;
        float frameTimer;
        float totalTime;
        float fps;
        int currentFrameIndex;

        IRecAudio soundRecorder;
        Texture2D frameBuffer;

        IFFmpegCallbacksHandler[] handlers;

        public RecAudioSource AudioSource => audioSource;
        public bool OverrideResolution => initialResultion.x > 0 || initialResultion.y > 0;

        public FFmpegRECState State { get; private set; }

        public float WritingProgress { get; private set; }

        public float WritingPercent => WritingProgress * 100;

        RGBToVideo command;
        internal RGBToVideo Command
        {
            get
            {
                if(State != FFmpegRECState.Processing)
                {
                    throw new Exception("Command is available only during Processing State.");
                }
                return command;
            }
        }

        FFmpegProcess ffmpegProcess;
        public FFmpegProcess FFmpegProcess
        {
            get
            {
                if (State != FFmpegRECState.Processing)
                {
                    throw new Exception("FFmpegProcess is available only during Processing State.");
                }
                return ffmpegProcess;
            }
        }

        public bool IsActive
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        readonly Queue<Frame> frames = new Queue<Frame>();
        readonly object framesLock = new object();

        //PUBLIC INTERFACE
        //------------------------------

        public void StartREC(params IFFmpegCallbacksHandler[] handlers)
        {
            string outputVideoPath = Path.Combine(Application.temporaryCachePath, VIDEO_NAME);
            StartREC(outputVideoPath, handlers);
        }

        public void StartREC(RecAudioSource audioSource, params IFFmpegCallbacksHandler[] handlers)
        {
            this.audioSource = audioSource;
            StartREC(handlers);
        }

        public void StartREC(string outputVideoPath, params IFFmpegCallbacksHandler[] handlers)
        {
            if (State != FFmpegRECState.Idle)
            {
                Debug.LogWarning($"{nameof(FFmpegREC)} State is {State}.");
                return;
            }

            this.outputVideoPath = outputVideoPath;
            ValidateOutputPath();

            Init();

#if !UNITY_EDITOR
            if (OverrideResolution)
            {
                Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
            }
            else
#endif
            {
                resolution = initialResultion;
            }

            frameBuffer = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false, true);
            camRect = new Rect(Vector2.zero, resolution);
            Camera = GetComponent<Camera>();
            Camera.targetTexture = new RenderTexture(resolution.x, resolution.y, 32);
            initialClearFlags = Camera.clearFlags;
            Camera.clearFlags = clearFlags;
            initialBackgroundColor = Camera.backgroundColor;
            Camera.backgroundColor = backgroundColor;
            startTime = Time.time;
            frameInterval = 1.0f / targetFPS;
            frameTimer = frameInterval;
            currentFrameIndex = 0;
            WritingProgress = 0;

            if (audioSource != RecAudioSource.None)
            {
                soundRecorder.StartRecording();
            }

            var handlersList = handlers == null ? new List<IFFmpegCallbacksHandler>() : new List<IFFmpegCallbacksHandler>(handlers);
            handlersList.Add(this);
            this.handlers = handlersList.ToArray();

            State = FFmpegRECState.Capturing;

            Task.Run(RunWritingThreads);
        }
        

        public void StopREC()
        {
            if (State == FFmpegRECState.Capturing)
            {
#if !UNITY_EDITOR
                if (OverrideResolution)
                {
                    //Return to initial screen resolution
                    Screen.SetResolution(initialResultion.x, initialResultion.y, Screen.fullScreen);
                }
#endif

                Camera.clearFlags = initialClearFlags;
                Camera.backgroundColor = initialBackgroundColor;

                totalTime = Time.time - startTime;
                fps = currentFrameIndex / totalTime;

                if (audioSource != RecAudioSource.None)
                {
                    soundRecorder.StopRecording(audioPath);
                }

                CreateVideo();

                State = FFmpegRECState.Processing;
            }
        }

        public void Cancel()
        {
            if(State == FFmpegRECState.Processing)
            {
                FFmpeg.Cancel(FFmpegProcess.Id);
            }

            Finish();
        }

        //FFMPEG CALLBACKS INTERFACE
        //------------------------------

        public override void OnFinish(long executionId)
        {
            base.OnFinish(executionId);

            Finish();
        }

        //INTERNAL IMPLEMENTATION
        //------------------------------

        void ValidateOutputPath()
        {
            if (string.IsNullOrWhiteSpace(outputVideoPath))
            {
                throw new ArgumentException("Empty Output Path.");
            }
            else
            {
                string outputDirectory = Path.GetDirectoryName(outputVideoPath);

                if (!Directory.Exists(outputDirectory))
                {
                    throw new DirectoryNotFoundException($"Invalid Output Path. Directory not found: {outputDirectory}");
                }
            }
        }

        void Init()
        {
            Clear();

            //Paths initialization
            cacheDir = Path.Combine(Application.temporaryCachePath, CACHE_DIRECTORY_NAME, Path.GetFileNameWithoutExtension(outputVideoPath));
            Directory.CreateDirectory(cacheDir);
            Debug.Log($"Cache directory created: {cacheDir}");
            imgeFilePathFormat = Path.Combine(cacheDir, FILE_FORMAT);
            audioPath = Path.Combine(cacheDir, Path.GetFileNameWithoutExtension(outputVideoPath) + AUDIO_EXTENSION);

            initialResultion = new Vector2Int(Screen.width, Screen.height);

            //Sound source initialization
            if (audioSource == RecAudioSource.Mic)
            {
                soundRecorder = GetComponent<RecMicAudio>();
            }
            else if (audioSource == RecAudioSource.System)
            {
                soundRecorder = GetComponent<RecSystemAudio>();
            }
            else if (audioSource == RecAudioSource.None)
            {
                soundRecorder = null;
            }
        }

        void OnPostRender()
        {
            if (State == FFmpegRECState.Capturing && (frameTimer += Time.deltaTime) > frameInterval)
            {
                frameTimer -= frameInterval;

                var currentRenderTexture = RenderTexture.active;
                RenderTexture.active = Camera.targetTexture;

                frameBuffer.ReadPixels(camRect, 0, 0, false);

                RenderTexture.active = currentRenderTexture;

                frames.Enqueue(new Frame
                {
                    Index = currentFrameIndex++,
                    RawData = frameBuffer.GetRawTextureData()
                });
            }
        }

        void RunWritingThreads()
        {
            Debug.Log($"Start {writingThreadsCount} Tasks.");

            try
            {
                Task[] writingTasks = new Task[writingThreadsCount];

                const int TO_MS = 1000;
                int intervalMS = Mathf.RoundToInt(frameInterval * TO_MS);

                for (int i = 0; i < writingTasks.Length; ++i)
                {
                    writingTasks[i] = WriteFramesTask(i, intervalMS);
                }

                Task.WaitAll(writingTasks, cancellation.Token);
            }
            catch(Exception ex)
            {
                Debug.LogError($"RunWritingThreads Error: {ex}");
                throw;
            }

            Debug.Log("Exit all REC Tasks.");
        }

        async Task WriteFramesTask(int taskIndex, int intervalMS)
        {
            Debug.Log($"Started REC Task {taskIndex}.");

            try
            {
                while (State < FFmpegRECState.Processing || frames.Count > 0)
                {
                    if(cancellation.IsCancellationRequested)
                    {
                        Debug.Log($"Canceled REC Task {taskIndex}.");
                        break;
                    }

                    lock(framesLock)
                    {
                        if(frames.Count > 0)
                        {
                            var frameData = frames.Dequeue();
                            string frameDataPath = GetImgeFilePath(frameData.Index);
                            File.WriteAllBytes(frameDataPath, frameData.RawData);
                        }
                    }

                    await Task.Delay(intervalMS);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"REC Thread Error: {ex}");
                throw;
            }

            Debug.Log($"Exit REC Task {taskIndex}.");
        }

        string GetImgeFilePath(int index)
        {
            return string.Format(imgeFilePathFormat, index.ToString(NUMBER_FORMAT));
        }

        void CreateVideo()
        {
            string frameInput = string.Format(imgeFilePathFormat, FFMPEG_NUMBER_FORMAT);
            command = new RGBToVideo(frameInput, audioPath, outputVideoPath, resolution, fps, totalTime, audioSource, quality);
            ffmpegProcess = FFmpeg.Execute(command.ToString(), handlers);
        }

        void Clear()
        {
            //RAM
            frames.Clear();

            //ROM
            if (Directory.Exists(cacheDir))
            {
                Directory.Delete(cacheDir, true);
            }
        }

        void Finish()
        {
            State = FFmpegRECState.Idle;
            Clear();
        }

        //------------------------------
    }
}