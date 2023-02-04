using GigaFileBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    abstract class ChooseFileView : MonoBehaviour
    {
        [SerializeField]
        Text selectedPath = null;

        [SerializeField]
        Button chooseFileButton = null;

        public event Action<string> OnChooseFileEvent = delegate { };

        public string InputPath
        {
            get => selectedPath.text;
            set => selectedPath.text = value;
        }

        public bool IsFileSelected => File.Exists(InputPath);

        protected abstract List<string> Extensions { get; }

        void Awake()
        {
            chooseFileButton.onClick.AddListener(OnChooseFileButton);
        }

        void OnChooseFileButton()
        {
            FileBrowser.ChooseFile(
                OnChooseFile,
                extensionsFilter: Extensions,
                color: Color.green);
        }

        void OnChooseFile(string path)
        {
            InputPath = path;
            OnChooseFileEvent(path);
        }
    }
}