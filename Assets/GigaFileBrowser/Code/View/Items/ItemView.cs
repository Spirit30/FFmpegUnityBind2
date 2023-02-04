using UnityEngine;
using UnityEngine.UI;

namespace GigaFileBrowser.Internal.View
{
    public abstract class ItemView : MonoBehaviour
    {
        [SerializeField]
        LimitedTextView nameLable = null;

        [SerializeField]
        RectTransform selectionOverlay = null;

        protected LookupMode mode;

        public  string Path { get; private set; }

        public bool IsSelected => selectionOverlay.gameObject.activeSelf;

        public virtual void Init(LookupMode mode, string path)
        {
            this.mode = mode;
            Path = path;
        }

        protected void SetText(string text)
        {
            nameLable.Text = text;
        }

        public void Select(bool flag)
        {
            selectionOverlay.gameObject.SetActive(flag);
        }

        protected abstract void OnClick();
    }
}