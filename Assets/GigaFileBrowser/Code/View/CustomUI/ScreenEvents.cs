using System;

namespace UnityEngine.UI
{
    public class ScreenEvents : MonoBehaviour
    {
        public event Action<CanvasOrientation> OnCanvasOrientationChange = delegate { };

        int previousWidth;

        void Update()
        {
            if(Screen.width != previousWidth)
            {
                previousWidth = Screen.width;
                CanvasOrientation canvasOrientation = Screen.width >= Screen.height ? CanvasOrientation.Lanscape : CanvasOrientation.Portrait;
                OnCanvasOrientationChange(canvasOrientation);
            }
        }
    }
}