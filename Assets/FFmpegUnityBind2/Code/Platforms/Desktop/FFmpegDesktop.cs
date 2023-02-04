#if ENABLE_IL2CPP
using IL2CPP.Process.Client;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if ENABLE_IL2CPP
using Execution = System.Tuple<IL2CPP.Process.Client.IL2CPP_Process, System.Collections.Generic.List<FFmpegUnityBind2.IFFmpegCallbacksHandler>>;
#else
using Execution = System.Tuple<System.Diagnostics.Process, System.Collections.Generic.List<FFmpegUnityBind2.IFFmpegCallbacksHandler>>;
#endif

namespace FFmpegUnityBind2.Desktop
{
    using Internal;
    using Utils;

    public static class FFmpegDesktop
    {
        const long START_EXECUTION_ID_LEVEL = 3000;
        static long executionIdBuffer = START_EXECUTION_ID_LEVEL;
        static long NextExecutionId => ++executionIdBuffer;
        
        static string ExecutableRelativePath
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return "Desktop/Win/ffmpeg.exe";
                }
                else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                {
                    return "Desktop/Mac/ffmpeg";
                }

                throw FFmpeg.PlatformNotSupportedException();
            }
        }

        static string executablePath;
        static string ExecutablePath
        {
            get
            {
                if (executablePath == null)
                {
                    executablePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(ExecutableRelativePath));
                }
                return executablePath;
            }
        }

        static string IL2CPP_ProcessName
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return "IL2CPP-Process.exe";
                }
                else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                {
                    return "IL2CPP-Process";
                }

                throw FFmpeg.PlatformNotSupportedException();
            }
        }

        static string il2cpp_ProcessRelativePath;
        static string IL2CPP_ProcessRelativePath
        {
            get
            {
                if (il2cpp_ProcessRelativePath == null)
                {
                    il2cpp_ProcessRelativePath = Path.Combine(Path.GetDirectoryName(ExecutableRelativePath), IL2CPP_ProcessName);
                }
                return il2cpp_ProcessRelativePath;
            }
        }

        static string il2cpp_ProcessPath;
        static string IL2CPP_ProcessPath
        {
            get
            {
                if (il2cpp_ProcessPath == null)
                {
                    il2cpp_ProcessPath = Path.Combine(Path.GetDirectoryName(ExecutablePath), IL2CPP_ProcessName);
                }
                return il2cpp_ProcessPath;
            }
        }

        static bool IsIL2CPP_ProcessPermission { get; set; }

        static bool HasExecutable => File.Exists(ExecutablePath);

        static readonly Dictionary<long, Execution> executions = new Dictionary<long, Execution>();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        [System.Runtime.InteropServices.DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
        static extern int sys_chmod(string path, uint mode);
#endif

        public static long Execute(string command, List<IFFmpegCallbacksHandler> callbacksHandlers)
        {
            long executionId = NextExecutionId;

            Task.Run(async () =>
            {
                long threadExecutionId = executionId;

                try
                {
                    for (int i = 0; i < callbacksHandlers.Count; i++)
                    {
                        var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnStart);
                        EventDispatcher.Invoke(@event);
                    }

                    await InitExecutable();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                    //Grant permission to ffmpeg binary
                    sys_chmod(ExecutablePath, 755);
#endif

#if ENABLE_IL2CPP
                    var process = new IL2CPP_Process();
#else
                    var process = new Process();
#endif

#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) && ENABLE_IL2CPP
                    if(!IsIL2CPP_ProcessPermission)
                    {
                        IsIL2CPP_ProcessPermission = true;
                        sys_chmod(IL2CPP_ProcessPath, 755);
                    }
#endif

                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(ExecutablePath);
                    process.StartInfo.FileName = ExecutablePath;
                    process.StartInfo.Arguments = command;

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            for (int i = 0; i < callbacksHandlers.Count; i++)
                            {
                                var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnLog, e.Data);
                                EventDispatcher.Invoke(@event);
                            }
                        }
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            if (e.Data.ToLower().Contains("error"))
                            {
                                for (int i = 0; i < callbacksHandlers.Count; i++)
                                {
                                    var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnError, e.Data);
                                    EventDispatcher.Invoke(@event);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < callbacksHandlers.Count; i++)
                                {
                                    var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnLog, e.Data);
                                    EventDispatcher.Invoke(@event);
                                }
                            }
                        }
                    };

                    process.Start();

                    var execution = new Execution(process, callbacksHandlers);
                    executions.Add(threadExecutionId, execution);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    if(process.ExitCode == 0)
                    {
                        for (int i = 0; i < callbacksHandlers.Count; i++)
                        {
                            var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnSuccess);
                            EventDispatcher.Invoke(@event);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < callbacksHandlers.Count; i++)
                        {
                            var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnFail);
                            EventDispatcher.Invoke(@event);
                        }
                    }

                    process.Close();
                }
                catch(Exception ex)
                {
                    for (int i = 0; i < callbacksHandlers.Count; i++)
                    {
                        var @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnError, ex.Message);
                        EventDispatcher.Invoke(@event);

                        @event = new Event(callbacksHandlers[i], threadExecutionId, EventType.OnFail);
                        EventDispatcher.Invoke(@event);
                    }

                    throw ex;
                }
            });

            return executionId;
        }

        public static void Cancel(long executionId)
        {
            if (executions.ContainsKey(executionId))
            {
                var execution = executions[executionId];
                var process = execution.Item1;
                var callbacksHandlers = execution.Item2;

                try
                {
                    process?.Kill();
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex);
                }

                for (int i = 0; i < callbacksHandlers.Count; i++)
                {
                    var @event = new Event(callbacksHandlers[i], executionId, EventType.OnCanceled);
                    EventDispatcher.Invoke(@event);
                }
            }
            else
            {
                Debug.LogWarning($"Process with execution Id {executionId} is not found.");
            }
        }

        public static void Init()
        {
#if ENABLE_IL2CPP
            Debug.Log($"IL2CPP - {true}");

            string[] relativePaths = new string[]
            { 
                IL2CPP_ProcessRelativePath,
                ExecutableRelativePath
            };

            string[] paths = new string[]
            {
                IL2CPP_ProcessPath,
                ExecutablePath
            };

            FileUnpacker.UnpackFiles(relativePaths, paths);
#else
            Debug.Log($"IL2CPP - {false}");
            FileUnpacker.UnpackFile(ExecutableRelativePath, ExecutablePath);
#endif
        }

        static async Task InitExecutable()
        {
            if (!HasExecutable)
            {
                Dispatcher.Invoke(() => Init());
                await WaitUntilHasExecutable();
            }
        }

        static async Task WaitUntilHasExecutable()
        {
            while (!HasExecutable)
            {
                await Task.Delay(1000);
            }
        }
    }
}