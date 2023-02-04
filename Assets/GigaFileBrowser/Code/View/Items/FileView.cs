using UnityEngine;
using UnityEngine.UI;

namespace GigaFileBrowser.Internal.View
{
    public class FileView : ItemView
    {
        [SerializeField]
        Button button = null;

        [SerializeField]
        LimitedTextView extensionView = null;

        public override void Init(LookupMode mode, string path)
        {
            base.Init(mode, path);
            SetText(System.IO.Path.GetFileNameWithoutExtension(path));
            extensionView.Text = System.IO.Path.GetExtension(path);
        }

        void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        protected override void OnClick()
        {
            if (mode >= LookupMode.ChooseFile)
            {
                FindObjectOfType<FileBrowserView>().Select(this);
            }
        }
    }
}