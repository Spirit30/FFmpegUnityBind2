using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace IL2CPP.Process.Client.Demo
{
    public class IL2CPP_ProcessDemo : MonoBehaviour
    {
        #region UI REFERENCES

        [SerializeField]
        Text workindDirectoryLable = null;

        [SerializeField]
        InputField processName = null;

        [SerializeField]
        InputField processArguments = null;

        [SerializeField]
        InputField processInput = null;

        [SerializeField]
        Text outputLable = null;

        [SerializeField]
        Button processStartButton = null;

        #endregion

        #region PROCESS REFERENCE

#if ENABLE_IL2CPP
        IL2CPP_Process process;
#else
        System.Diagnostics.Process process;
#endif

        #endregion

        #region DATA

        string executablePath;
        string arguments;

        #endregion

        #region VARIABLES

        Queue<string> input = new Queue<string>();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        #endregion

        #region UI EVENTS

        public void OnProcessInput(string input)
        {
            this.input.Enqueue(input);
        }

        public void OnProcessStartButton()
        {
            if (!string.IsNullOrWhiteSpace(processName.text))
            {
                executablePath = Path.Combine(workindDirectoryLable.text, processName.text);

                if (File.Exists(executablePath))
                {
                    arguments = processArguments.text;

                    Task.Run(() =>
                    {
                        try
                        {
                            Run();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            throw ex;
                        }
                    });

                    processName.interactable = false;
                    processArguments.interactable = false;
                    processInput.interactable = true;
                    processStartButton.interactable = false;
                }
                else
                {
                    Debug.LogError($"File is not found at: {executablePath}");
                }
            }
            else
            {
                Debug.LogError("Please set Process name.");
            }
        }

        #endregion

        #region UNITY EVENTS

        void Start()
        {
            workindDirectoryLable.text = Directory.GetCurrentDirectory();
            Application.quitting += OnQuit;
        }

        void OnQuit()
        {
            try
            {
                Debug.Log("On Quit");
                Finish();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        #endregion

        #region IMPLEMENTATION

        void Run()
        {
#if ENABLE_IL2CPP
            process = new IL2CPP_Process();
            string workingDirectory = Directory.GetCurrentDirectory();
            bool isIL2CPP = true;
#else
            process = new System.Diagnostics.Process();
            string workingDirectory = Path.GetDirectoryName(executablePath);
            bool isIL2CPP = false;
#endif
            Debug.Log($"Is IL2CPP: {isIL2CPP}");

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = executablePath;
            process.StartInfo.Arguments = arguments;

            Task.Run(async () =>
            {
                try
                {
                    //Input sending loop
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        const int INTERVAL = 1000 / 30;
                        await Task.Delay(INTERVAL);

                        if (input.Count > 0)
                        {
                            process.StandardInput.WriteLine(input.Dequeue());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    throw ex;
                }
            }, cancellationTokenSource.Token);

            process.OutputDataReceived += (s, e) => Dispatcher.Invoke(() => OnOutput(e.Data));
            process.ErrorDataReceived += (s, e) => Dispatcher.Invoke(() => OnError(e.Data));

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            Debug.Log("Passed WaitForExit");

            Dispatcher.Invoke(() =>
            {
                processInput.interactable = false;
                OnOutput("Exit");
            });

            Finish();

            process.Close();
        }

        void Finish()
        {
            //Exit from Input sending loop & Output listening loop
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region UPDATE UI & OUTPUT

        void OnOutput(string output)
        {
            if (!string.IsNullOrWhiteSpace(output))
            {
                outputLable.text = output;
                Debug.Log(output);
            }
        }

        void OnError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                outputLable.text = $"<color=red>{error}</color>";
                Debug.LogError(error);
            }
        }

        #endregion
    }
}