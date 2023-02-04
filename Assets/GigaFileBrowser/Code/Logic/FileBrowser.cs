using GigaFileBrowser.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GigaFileBrowser
{
    public static class FileBrowser
    {
        #region PUBLIC API

        /// <summary>
        /// Choose one directory.
        /// </summary>
        /// <param name="onChoose">Callback with one directory path argument.</param>
        /// <param name="onCancel">Callback on canceled by user.</param>
        /// <param name="onError">Callback on error with exception message.</param>
        /// <param name="startLookupDirectory">Directory to open at start.</param>
        /// <param name="parent">RectTransform panel to open in. If null - separate Canvas will be created.</param>
        /// <param name="color">Color theme.</param>
        public static void ChooseDirectory(Action<string> onChoose, Action onCancel = null, Action<string> onError = null, string startLookupDirectory = null, RectTransform parent = null, Color color = default)
        {
            Open(LookupMode.ChooseDirectory, onChoose, null, onCancel, onError, startLookupDirectory, parent, color, null);
        }

        /// <summary>
        /// Choose several directories.
        /// </summary>
        /// <param name="onChoose">Callback with several directories paths argument.</param>
        /// <param name="onCancel">Callback on canceled by user.</param>
        /// <param name="onError">Callback on error with exception message.</param>
        /// <param name="startLookupDirectory">Directory to open at start.</param>
        /// <param name="parent">RectTransform panel to open in. If null - separate Canvas will be created.</param>
        /// <param name="color">Color theme.</param>
        public static void ChooseDirectories(Action<string[]> onChoose, Action onCancel = null, Action<string> onError = null, string startLookupDirectory = null, RectTransform parent = null, Color color = default)
        {
            Open(LookupMode.ChooseDirectories, null, onChoose, onCancel, onError, startLookupDirectory, parent, color, null);
        }

        /// <summary>
        /// Choose one file.
        /// </summary>
        /// <param name="onChoose">Callback with one file path argument.</param>
        /// <param name="onCancel">Callback on canceled by user.</param>
        /// <param name="onError">Callback on error with exception message.</param>
        /// <param name="startLookupDirectory">Directory to open at start.</param>
        /// <param name="parent">RectTransform panel to open in. If null - separate Canvas will be created.</param>
        /// <param name="color">Color theme.</param>
        /// <param name="extensionsFilter">Extensions of files to search. Format: ".txt" or ".png" or ".mp4" etc. Only files with extensions included to extensionsFilter list will be visible. If null or empty - files with all extensions will be visible.</param>
        public static void ChooseFile(Action<string> onChoose, Action onCancel = null, Action<string> onError = null, string startLookupDirectory = null, RectTransform parent = null, Color color = default, List<string> extensionsFilter = null)
        {
            Open(LookupMode.ChooseFile, onChoose, null, onCancel, onError, startLookupDirectory, parent, color, extensionsFilter);
        }

        /// <summary>
        /// Choose several files.
        /// </summary>
        /// <param name="onChoose">Callback with several files paths argument.</param>
        /// <param name="onCancel">Callback on canceled by user.</param>
        /// <param name="onError">Callback on error with exception message.</param>
        /// <param name="startLookupDirectory">Directory to open at start.</param>
        /// <param name="parent">RectTransform panel to open in. If null - separate Canvas will be created.</param>
        /// <param name="color">Color theme.</param>
        /// <param name="extensionsFilter">Extensions of files to search. Format: ".txt" or ".png" or ".mp4" etc. Only files with extensions included to extensionsFilter list will be visible. If null or empty - files with all extensions will be visible.</param>
        public static void ChooseFiles(Action<string[]> onChoose, Action onCancel = null, Action<string> onError = null, string startLookupDirectory = null, RectTransform parent = null, Color color = default, List<string> extensionsFilter = null)
        {
            Open(LookupMode.ChooseFiles, null, onChoose, onCancel, onError, startLookupDirectory, parent, color, extensionsFilter);
        }

        #endregion

        #region INTERNAL INTERFACE

        static void Open(LookupMode mode, Action<string> onChooseOne, Action<string[]> onChooseMany, Action onCancel, Action<string> onError, string startLookupDirectory, RectTransform parent, Color color, List<string> extensionsFilter)
        {
            var view = FileBrowserFactory.Open(parent);
            view.Init(mode, (RectTransform)(parent ? view.transform : view.transform.parent), onChooseOne, onChooseMany, onCancel, onError, color, extensionsFilter);
            view.UpdateView(Directory.Exists(startLookupDirectory) ? startLookupDirectory : Application.persistentDataPath);
        }

        #endregion
    }
}