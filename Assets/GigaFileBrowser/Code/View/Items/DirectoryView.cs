using UnityEngine;
using UnityEngine.UI;

namespace GigaFileBrowser.Internal.View
{
    public class DirectoryView : ItemView
    {
        [SerializeField]
        ButtonWithLongPress longPressButton = null;

        public override void Init(LookupMode mode, string path)
        {
            base.Init(mode, path);
            SetText(path);
        }

        protected override void OnClick()
        {
            FindObjectOfType<FileBrowserView>().UpdateView(Path);
        }

        void Awake()
        {
            longPressButton.OnClick += OnClick;
            longPressButton.OnLongPress += OnLongPressButton;
        }

        void OnLongPressButton()
        {
            if(mode <= LookupMode.ChooseDirectories)
            {
                FindObjectOfType<FileBrowserView>().Select(this);
            }
        }
    }
}