using FFmpegUnityBind2.Android;
using FFmpegUnityBind2.Desktop;
using FFmpegUnityBind2.Internal;
using FFmpegUnityBind2.IOS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FFmpegUnityBind2
{
    public static class FFmpeg
    {
        /// <summary>
        /// Should output to Debug.Log or not.
        /// </summary>
        public static bool IsPrintingLog { get; set; } = true;

        static List<IFFmpegCallbacksHandler> _callbacksHandlers;

        /// <summary>
        /// Call from Main Thread only. Can be called ahead of execution.
        /// Or will be performed automatically on first execution.
        /// </summary>
        public static void Init()
        {
            if (!EventDispatcherFactory.GetInstance())
            {
                EventDispatcherFactory.CreateInstance();
            }
        }

        /// <summary>
        /// Main and only FFmpeg lib Interface. One superpowerfull method for all needs.
        /// </summary>
        /// <param name="command">Particular command. Arguments "https://ffmpeg.org"</param>
        /// <param name="callbacksHandlers">Handlers for FFmpeg callback events.</param>
        /// <returns></returns>
        public static FFmpegProcess Execute(string command, List<IFFmpegCallbacksHandler> callbacksHandlers)
        {
            if (IsPrintingLog)
            {
                Debug.Log($"Command: {command}");
            }

            Init();

            long executionId;
            var processInternal = new FFmpegProgress();

            callbacksHandlers.Add(processInternal);

            if (IsPrintingLog)
            {
                Debug.Log("Print FFmpeg Log.");
                callbacksHandlers.Add(new FFmpegCallbacksHandlerDebug());
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                executionId = FFmpegAndroid.Execute(command, callbacksHandlers);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                executionId = FFmpegIOS.Execute(command, callbacksHandlers);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXPlayer
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                executionId = FFmpegDesktop.Execute(command, callbacksHandlers);
            }
            else
            {
                throw PlatformNotSupportedException();
            }

            return new FFmpegProcess(executionId, processInternal);
        }

        /// <summary>
        /// Main and only FFmpeg lib Interface. One superpowerfull method for all needs.
        /// </summary>
        /// <param name="command">Particular command. Arguments "https://ffmpeg.org"</param>
        /// <param name="callbacksHandlers">Handlers for FFmpeg callback events.</param>
        /// <returns></returns>
        public static FFmpegProcess Execute(string command, params IFFmpegCallbacksHandler[] callbacksHandlers)
        {
            _callbacksHandlers = new List<IFFmpegCallbacksHandler>(callbacksHandlers);

            return Execute(command, _callbacksHandlers);
        }

        /// <summary>
        /// Cancelation Interface. Use FFmpegProcess.Id given by FFmpeg.Execute.
        /// </summary>
        /// <param name="executionId"></param>
        public static void Cancel(long executionId)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                FFmpegAndroid.Cancel(executionId);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                FFmpegIOS.Cancel(executionId);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXPlayer
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                FFmpegDesktop.Cancel(executionId);
            }
            else
            {
                throw PlatformNotSupportedException();
            }
        }

        /// <summary>
        /// Throw to protect from illegal platform usage.
        /// </summary>
        /// <returns>Platform Not Supported Exception</returns>
        public static PlatformNotSupportedException PlatformNotSupportedException()
        {
            return new PlatformNotSupportedException(
                $"{Application.platform} platform is not supported.");
        }
    }
}