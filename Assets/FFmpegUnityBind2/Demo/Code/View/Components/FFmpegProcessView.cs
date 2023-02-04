using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class FFmpegProcessView : MonoBehaviour
    {
        [SerializeField]
        Text lable = null;

        [SerializeField]
        Image progressBar = null;

        [SerializeField]
        RectTransform progressBarPanel = null;

        [SerializeField]
        Text doneLable = null;

        public FFmpegProcess FFmpegProcess { get; private set; }

        public FFmpegProcessView Duplicate(FFmpegProcess ffmpegProcess)
        {
            var instance = Instantiate(this, transform.parent);
            instance.FFmpegProcess = ffmpegProcess;
            instance.lable.text = $"Process Id: {ffmpegProcess.Id}";
            instance.transform.SetSiblingIndex(0);
            instance.gameObject.SetActive(true);
            return instance;
        }

        void Update()
        {
            if(FFmpegProcess.IsDone)
            {
                doneLable.gameObject.SetActive(true);
                progressBarPanel.gameObject.SetActive(false);
                enabled = false;
            }
            else
            {
                progressBar.fillAmount = FFmpegProcess.Progress;
            }
        }
    }
}