using GigaFileBrowser.Internal.View;
using UnityEngine;

namespace GigaFileBrowser.Internal
{
    static class FileBrowserFactory
    {
        const string PREFAB_CANVAS = "GigaFileBrowserCanvas";
        const string PREFAB_PANEL = "GigaFileBrowserPanel";

        public static FileBrowserView Open(RectTransform parent)
        {
            string resourcePath = parent ? PREFAB_PANEL : PREFAB_CANVAS;
            var prefab = Resources.Load<GameObject>(resourcePath);
            return Object.Instantiate(prefab, parent).GetComponentInChildren<FileBrowserView>();
        }
    }
}