using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class Console : MonoBehaviour
    {
        static Console instance;
        static Console Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<Console>();
                }
                return instance;
            }
        }

        [SerializeField]
        ScrollRect scrollRect = null;

        [SerializeField]
        ConsoleChunk consoleChunkOrigin = null;

        [SerializeField]
        VerticalLayoutGroup verticalLayoutGroup = null;

        RectTransform ChunksContentPanel => verticalLayoutGroup.transform as RectTransform;

        [SerializeField]
        float contentSizeOffset = 100.0f;

        const int TEXT_VERTS_LIMIT = 65000;
        const int MAX_CHARS_COUNT = TEXT_VERTS_LIMIT / 4 - 1;

        ConsoleChunk CurrentChunk { get; set; }

        readonly List<ConsoleChunk> chunks = new List<ConsoleChunk>();

        public static void WriteLine(string line)
        {
            Instance.PrintToOutput(line);
        }

        public static void WriteWarning(string line)
        {
            WriteLine($"<color=yellow>{line}</color>");
        }

        public static void WriteError(string line)
        {
            WriteLine($"<color=red>{line}</color>");
        }

        public static void Clear()
        {
            Instance.ClearInternal();
        }

#region FORCE UPDATE LAYOUT GROUP

        void Update()
        {
            UpdateChunksContentPanelSizeY();
            verticalLayoutGroup.enabled = false;
        }

        void LateUpdate()
        {
            verticalLayoutGroup.enabled = true;
        }

        #endregion

        void PrintToOutput(string line)
        {
            if (!CurrentChunk || (CurrentChunk.Length + line.Length) > MAX_CHARS_COUNT)
            {
                CreateNewChunk();
            }
            else
            {
                line = '\n' + line.Replace("\n", string.Empty);
            }

            CurrentChunk.Append(line);
        }

        void UpdateChunksContentPanelSizeY()
        {
            SetChunksContentPanelSizeY(chunks.Sum(c => c.Height) + contentSizeOffset);
        }

        void SetChunksContentPanelSizeY(float y)
        {
            var sizeDelta = ChunksContentPanel.sizeDelta;

            if (y != sizeDelta.y)
            {
                sizeDelta.y = y;
                ChunksContentPanel.sizeDelta = sizeDelta;
                scrollRect.normalizedPosition = Vector2.zero;
            }
        }

        void CreateNewChunk()
        {
            CurrentChunk = Instantiate(consoleChunkOrigin, ChunksContentPanel);
            CurrentChunk.gameObject.SetActive(true);
            chunks.Add(CurrentChunk);
        }

        void ClearInternal()
        {
            while(chunks.Count > 0)
            {
                DestroyImmediate(chunks[0].gameObject);
                chunks.RemoveAt(0);
            }

            SetChunksContentPanelSizeY(consoleChunkOrigin.Height);
        }
    }
}