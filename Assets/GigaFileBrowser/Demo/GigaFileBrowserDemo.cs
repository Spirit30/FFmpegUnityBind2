using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GigaFileBrowser.Demo
{
    public class GigaFileBrowserDemo : MonoBehaviour
    {
        #region REFERENCES

        [SerializeField]
        Text outputLable = null;

        [SerializeField]
        Button chooseDirectoryButton = null;

        [SerializeField]
        Button chooseDirectoriesButton = null;

        [SerializeField]
        Button chooseFileButton = null;

        [SerializeField]
        Button chooseFilesButton = null;

        [SerializeField]
        Color color = Color.red;

        [SerializeField]
        RectTransform parentPanel = null;

        [SerializeField]
        List<string> extensionsFilter = null;

        #endregion

        #region UNITY EVENTS

        void Awake()
        {
            chooseDirectoryButton.onClick.AddListener(OnChooseDirectory);
            chooseDirectoriesButton.onClick.AddListener(OnChooseDirectories);
            chooseFileButton.onClick.AddListener(OnChooseFile);
            chooseFilesButton.onClick.AddListener(OnChooseFiles);
        }

        #endregion

        #region UI EVENTS

        void OnChooseDirectory()
        {
            FileBrowser.ChooseDirectory(
                onChoose: OnChooseDirectory,
                onCancel: OnCancel,
                onError: OnError,
                parent: parentPanel,
                color: color);
        }

        void OnChooseDirectories()
        {
            FileBrowser.ChooseDirectories(
                onChoose: OnChooseDirectories,
                onCancel: OnCancel,
                onError: OnError,
                parent: parentPanel,
                color: color);
        }

        void OnChooseFile()
        {
            FileBrowser.ChooseFile(
                onChoose: OnChooseFile,
                onCancel: OnCancel,
                onError: OnError,
                parent: parentPanel,
                color: color,
                extensionsFilter: extensionsFilter);
        }

        void OnChooseFiles()
        {
            FileBrowser.ChooseFiles(
                onChoose: OnChooseFiles,
                onCancel: OnCancel,
                onError: OnError,
                parent: parentPanel,
                color: color,
                extensionsFilter: extensionsFilter);
        }

        #endregion

        #region CALLBACKS

        void OnChooseDirectory(string path)
        {
            outputLable.color = Color.white;
            outputLable.text = $"DIRECTORY\n{path}";
        }

        void OnChooseDirectories(string[] paths)
        {
            outputLable.color = Color.white;
            var stringBuilder = new StringBuilder("DIRECTORIES\n");

            foreach (string path in paths)
            {
                stringBuilder.Append(path).Append('\n');
            }

            outputLable.text = stringBuilder.ToString();
        }

        void OnChooseFile(string path)
        {
            outputLable.color = Color.white;
            outputLable.text = $"FILE\n{path}";
        }

        void OnChooseFiles(string[] paths)
        {
            outputLable.color = Color.white;
            var stringBuilder = new StringBuilder("FILES\n");

            foreach (string path in paths)
            {
                stringBuilder.Append(path).Append('\n');
            }

            outputLable.text = stringBuilder.ToString();
        }

        void OnError(string error)
        {
            outputLable.color = Color.red;
            outputLable.text = $"ERROR\n{error}";
        }

        void OnCancel()
        {
            outputLable.color = Color.white;
            outputLable.text = "CANCELED";
        }

        #endregion
    }
}