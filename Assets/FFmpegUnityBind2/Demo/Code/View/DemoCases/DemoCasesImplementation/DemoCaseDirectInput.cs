using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseDirectInput : DemoCaseView
    {
        [SerializeField]
        InputField directInput = null;

        protected override void OnExecuteButton()
        {
            ExecuteWithOutput(directInput.text);
        }
    }
}
