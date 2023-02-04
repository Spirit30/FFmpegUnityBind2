using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GigaFileBrowser.Internal.View
{
    public class FileBrowserView : MonoBehaviour
    {
        [SerializeField]
        Button backButton = null;

        [SerializeField]
        Button cancelButton = null;

        [SerializeField]
        Button chooseButton = null;

        [SerializeField]
        LimitedTextView currentDirectoryLable = null;

        [SerializeField]
        GridLayoutGroup contentContainer = null;

        [SerializeField]
        DirectoryView directoryViewPrefab = null;

        [SerializeField]
        FileView fileViewPrefab = null;

        [SerializeField]
        ScreenEvents screenEvents = null;

        [SerializeField]
        Vector2 landscapeCellSize = new Vector2(240, 240);

        [SerializeField]
        Vector2 portraitCellSize = new Vector2(400, 400);

        LookupMode mode;
        RectTransform root;
        Action<string> onChooseOne;
        Action<string[]> onChooseMany;
        Action onCancel;
        Action<string> onError;
        Color color;
        List<string> extensionsFilter;

        DirectoryInfo currentDirectory;

        public void Init(LookupMode mode, RectTransform root, Action<string> onChooseOne, Action<string[]> onChooseMany, Action onCancel, Action<string> onError, Color color, List<string> extensionsFilter)
        {
            this.mode = mode;
            this.root = root;
            this.onChooseOne = onChooseOne;
            this.onChooseMany = onChooseMany;
            this.onCancel = onCancel;
            this.onError = onError;
            this.color = color;

            if (extensionsFilter != null && extensionsFilter.Count > 0)
            {
                this.extensionsFilter = FileExtensionsUtil.Permute(extensionsFilter);
            }
        }

        public void UpdateView(string directoryPath)
        {
            try
            {
                List<string> paths = new List<string>(Directory.GetDirectories(directoryPath));

                currentDirectoryLable.Text = directoryPath;
                currentDirectory = new DirectoryInfo(directoryPath);
                backButton.interactable = currentDirectory.Parent != null;

                if (mode >= LookupMode.ChooseFile)
                {
                    IEnumerable<string> files = Directory.GetFiles(directoryPath);

                    if (extensionsFilter != null && extensionsFilter.Count > 0)
                    {
                        files = files.Where(f => extensionsFilter.Contains(Path.GetExtension(f)));
                    }

                    paths.AddRange(files);
                }

                UpdateView(paths);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                OnError(ex.Message);
            }
        }

        void UpdateView(IEnumerable<string> paths)
        {
            ClearView();

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    Instantiate(directoryViewPrefab, contentContainer.transform).Init(mode, path);
                }
                else if(File.Exists(path))
                {
                    Instantiate(fileViewPrefab, contentContainer.transform).Init(mode, path);
                }
            }

            if (color != default)
            {
                UpdateColor();
            }
        }

        void UpdateColor()
        {
            foreach (var graphic in GetComponentsInChildren<Graphic>(true))
            {
                graphic.color *= color;
            }
        }

        public void Select(DirectoryView  directoryView)
        {
            if (mode == LookupMode.ChooseDirectory)
            {
                foreach (var otherDirectoryView in contentContainer.GetComponentsInChildren<DirectoryView>().Where(d => d != directoryView))
                {
                    otherDirectoryView.Select(false);
                }
            }

            directoryView.Select(!directoryView.IsSelected);

            chooseButton.interactable = directoryView.IsSelected || contentContainer.GetComponentsInChildren<DirectoryView>().Any(d => d.IsSelected);
        }

        public void Select(FileView fileView)
        {
            if(mode == LookupMode.ChooseFile)
            {
                foreach (var otherFileView in contentContainer.GetComponentsInChildren<FileView>().Where(f => f != fileView))
                {
                    otherFileView.Select(false);
                }
            }

            fileView.Select(!fileView.IsSelected);

            chooseButton.interactable = fileView.IsSelected || contentContainer.GetComponentsInChildren<FileView>().Any(f => f.IsSelected);
        }

        void Awake()
        {
            backButton.onClick.AddListener(OnBackButton);
            cancelButton.onClick.AddListener(OnCacnelButton);
            chooseButton.onClick.AddListener(OnChooseButton);
            screenEvents.OnCanvasOrientationChange += OnCanvasOrientationChange;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (backButton.interactable)
                {
                    OnBackButton();
                }
                else
                {
                    OnCacnelButton();
                }
            }
        }

        void OnCanvasOrientationChange(CanvasOrientation orientation)
        {
            contentContainer.cellSize = orientation == CanvasOrientation.Portrait ? portraitCellSize : landscapeCellSize;
        }

        void ClearView()
        {
            while (contentContainer.transform.childCount > 0)
            {
                DestroyImmediate(contentContainer.transform.GetChild(0).gameObject);
            }
        }

        void OnBackButton()
        {
            chooseButton.interactable = false;
            UpdateView(currentDirectory.Parent.FullName);
        }

        void OnCacnelButton()
        {
            Delete();
            onCancel?.Invoke();
        }

        void OnChooseButton()
        {
            Choose();
            Delete();
        }

        void Choose()
        {
            switch (mode)
            {
                case LookupMode.ChooseDirectory:
                    onChooseOne(contentContainer.GetComponentsInChildren<DirectoryView>().Single(d => d.IsSelected).Path);
                    break;

                case LookupMode.ChooseDirectories:
                    onChooseMany(contentContainer.GetComponentsInChildren<DirectoryView>().Where(d => d.IsSelected).Select(d => d.Path).ToArray());
                    break;

                case LookupMode.ChooseFile:
                    onChooseOne(contentContainer.GetComponentsInChildren<FileView>().Single(f => f.IsSelected).Path);
                    break;

                case LookupMode.ChooseFiles:
                    onChooseMany(contentContainer.GetComponentsInChildren<FileView>().Where(f => f.IsSelected).Select(f => f.Path).ToArray());
                    break;

                default:
                    throw new ArgumentException($"Unknown lookup mode: {mode}");
            }
        }

        void OnError(string error)
        {
            onError?.Invoke(error);
        }

        void Delete()
        {
            DestroyImmediate(root.gameObject);
        }
    }
}