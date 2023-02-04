using IL2CPP.Process.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using EventType = IL2CPP.Process.Shared.EventType;

namespace IL2CPP.Process.Client
{
    public class IL2CPP_Process
    {
#if UNITY_STANDALONE_WIN
        const string IL2CPP_PROCESS_NAME = "IL2CPP-Process.exe";
#elif UNITY_STANDALONE_OSX
        const string IL2CPP_PROCESS_NAME = "IL2CPP-Process";
#else
        const string IL2CPP_PROCESS_NAME = "NOT SUPPORTED";
#endif
        string il2cppProcessPath;
        DirectoryInfo serverDirectory;

        public StartInfo StartInfo { get; } = new StartInfo();

        public StandardInputHandler StandardInput { get; } = new StandardInputHandler();
        public event Action<object, DataReceivedEventArgs> OutputDataReceived = delegate { };
        public event Action<object, DataReceivedEventArgs> ErrorDataReceived = delegate { };

        public class StandardInputHandler
        {
            public DirectoryInfo serverDirectory;
            bool isActive;

            public void Activate(DirectoryInfo serverDirectory)
            {
                this.serverDirectory = serverDirectory;
                isActive = true;
            }

            public void Deactivate()
            {
                isActive = false;
            }

            public void WriteLine(string line)
            {
                if (isActive)
                {
                    EventFactory.FireEvent(EventType.INPUT, line, serverDirectory);
                }
            }
        }

        public class DataReceivedEventArgs : EventArgs
        {
            public string Data { get; set; }

            public DataReceivedEventArgs(string data)
            {
                Data = data;
            }
        }

        EventListener eventListener;
        bool shouldExit;

        public int ExitCode => shouldExit ? 0 : -1;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        internal bool Start()
        {
            try
            {
                Debug.Log("Start IL2CPP Process.");

                serverDirectory = new DirectoryInfo(StartInfo.WorkingDirectory);
                il2cppProcessPath = Path.Combine(serverDirectory.FullName, IL2CPP_PROCESS_NAME);

                if (File.Exists(il2cppProcessPath))
                {
                    ClearAllEvents();

                    string startInfoJson = JsonConvert.SerializeObject(StartInfo, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    EventFactory.FireEvent(EventType.INPUT, startInfoJson, serverDirectory);

                    Dispatcher.Invoke(() =>
                    {
                        Application.OpenURL("file://" + il2cppProcessPath);
                        StandardInput.Activate(serverDirectory);
                    });

                    return true;
                }
                else
                {
                    Debug.LogError("File not found at: " + il2cppProcessPath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        internal void BeginOutputReadLine()
        {
            eventListener = new EventListener(serverDirectory, cancellationTokenSource.Token, EventType.OUTPUT_LOG, EventType.OUTPUT_ERROR, EventType.OUTPUT_EXIT);
            eventListener.OnEvents += EventListener_OnEvents;
        }

        internal void BeginErrorReadLine()
        {
            if (eventListener == null)
            {
                Debug.LogError("You should call \"BeginOutputReadLine\" first.");
            }
            else
            {
                Debug.Log("IL2CPP_Process server is always read errors if read output (See source code).");
            }
        }

        internal void WaitForExit()
        {
            while (!shouldExit)
            {
                Debug.Log("Waiting For Exit...");
                Thread.Sleep(1000);
            }
            Debug.Log("Exit IL2CPP Process");
        }

        internal void Close()
        {
            //Stop sending input
            StandardInput?.Deactivate();

            //Stop reading output
            cancellationTokenSource.Cancel();

            //Remove all event files.
            ClearAllEvents();
        }

        internal void Kill()
        {
            OnExitEvent();
            Close();
        }

        void ClearAllEvents()
        {
            foreach (var @event in EventListener.ReadEvents(serverDirectory, EventType.INPUT, EventType.OUTPUT_LOG, EventType.OUTPUT_ERROR, EventType.OUTPUT_EXIT))
            {
                File.Delete(@event.path);
            }
        }

        void EventListener_OnEvents(List<Shared.Event> events)
        {
            foreach (var @event in events)
            {
                switch (@event.eventType)
                {
                    case EventType.OUTPUT_LOG:
                        OutputDataReceived(this, new DataReceivedEventArgs(@event.message));
                        Debug.Log("Raw Log Event: " + @event.message);
                        File.Delete(@event.path);
                        break;

                    case EventType.OUTPUT_ERROR:
                        ErrorDataReceived(this, new DataReceivedEventArgs(@event.message));
                        Debug.LogError("Raw Error Event: " + @event.message);
                        File.Delete(@event.path);
                        break;

                    case EventType.OUTPUT_EXIT:
                        Debug.Log("Exit Event.");
                        File.Delete(@event.path);
                        OnExitEvent();
                        break;
                }
            }
        }

        void OnExitEvent()
        {
            //Unlock thread in WaitForExit
            shouldExit = true;
        }
    }
}