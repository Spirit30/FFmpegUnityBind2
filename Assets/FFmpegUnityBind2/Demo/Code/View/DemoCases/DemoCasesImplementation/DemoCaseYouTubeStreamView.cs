using FFmpegUnityBind2.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseYouTubeStreamView : DemoCaseView
    {
        [SerializeField]
        FFmpegRECPool ffmpegRECPool = null;

        [SerializeField]
        InputField streamKeyField = null;

        [SerializeField]
        Button stopButton = null;

        [SerializeField]
        float partTime = 20.0f;

        static readonly WaitForEndOfFrame frame = new WaitForEndOfFrame();

        readonly Queue<string> readyVideoParts = new Queue<string>();
        FFmpegREC currentFFmpegREC;
        int currentPartIndex;
        float partTimer;
        bool isStreaming;
        bool isAlive;

        protected override void OnExecuteButton()
        {
            ValidateStreamKey();

            isStreaming = true;
            demoCaseSharedView.SetupSceneForStartCapturing(true);
            executeButton.interactable = false;
            stopButton.interactable = true;
        }

        protected override void Awake()
        {
            base.Awake();
            stopButton.onClick.AddListener(OnStopButton);

            isAlive = true;
            StartCoroutine(RecordingLoop());
            StartCoroutine(SendingLoop());
            stopButton.interactable = false;
        }

        void OnStopButton()
        {
            isStreaming = false;
            demoCaseSharedView.SetupSceneForStopCapturing(true);
            executeButton.interactable = true;
            stopButton.interactable = false;
        }

        IEnumerator RecordingLoop()
        {
            while (isAlive)
            {
                if (isStreaming)
                {
                    currentFFmpegREC = ffmpegRECPool.GetWhichIdle();
                    string partFilePath = GetPartFilePath(currentPartIndex++);
                    currentFFmpegREC.IsActive = true;
                    currentFFmpegREC.OnFinishEvent += OnFFmpegRECFinish;
                    currentFFmpegREC.StartREC(partFilePath, Handlers);
                    partTimer = partTime;

                    while(isStreaming && partTimer > 0)
                    {
                        partTimer -= Time.deltaTime;
                        yield return frame;
                    }

                    currentFFmpegREC.StopREC();
                    currentFFmpegREC.IsActive = false;
                }
                else
                {
                    yield return frame;
                }
            }
        }

        IEnumerator SendingLoop()
        {
            while (isAlive)
            {
                if (isStreaming && readyVideoParts.Count > 0 && !IsProcessing)
                {
                    string sendingPartPath = readyVideoParts.Dequeue();
                    var command = new YouTubeStreamCommand(sendingPartPath, streamKeyField.text);
                    ExecuteWithOutput(command);
                }

                yield return frame;
            }
        }

        void OnFFmpegRECFinish(long executionId, FFmpegCallbacksHandlerBase handler)
        {
            FFmpegREC ffmpegREC = (FFmpegREC)handler;
            ffmpegREC.OnFinishEvent -= OnFFmpegRECFinish;
            readyVideoParts.Enqueue(ffmpegREC.Command.OutputPathOrigin);
        }

        void ValidateStreamKey()
        {
            if(string.IsNullOrWhiteSpace(streamKeyField.text))
            {
                throw new Exception("Please fill Stream Key.");
            }
        }

        void OnDestroy()
        {
            isAlive = false;
        }

        static string GetPartFilePath(int index)
        {
            return Path.Combine(Application.temporaryCachePath, $"part_{index}.mp4");
        }
    }
}