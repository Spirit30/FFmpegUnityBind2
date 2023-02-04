using GigaFileBrowser;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    public class ChooseDirectory : MonoBehaviour
    {
        [SerializeField]
        Text selectedPath = null;

        [SerializeField]
        Button chooseDirectoryButton = null;

        public event Action<string> OnChooseDirectoryEvent = delegate { };

        public string InputPath
        {
            get => selectedPath.text;
            set => selectedPath.text = value;
        }

        public bool IsDirectorySelected => initialInputPathValue != InputPath && Directory.Exists(InputPath);
        string initialInputPathValue;

        void Awake()
        {
            initialInputPathValue = InputPath;
            chooseDirectoryButton.onClick.AddListener(OnChooseDirectoryButton);
        }

        void OnChooseDirectoryButton()
        {
            FileBrowser.ChooseDirectory(
                OnChooseDirectory,
                color: Color.green);
        }

        void OnChooseDirectory(string path)
        {
            InputPath = path;
            OnChooseDirectoryEvent(path);
        }
    }
}