using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    public class SpinningWheelView : MonoBehaviour
    {
        [SerializeField]
        Transform wheel = null;

        [SerializeField]
        float rotationSpeed = 1.0f;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            wheel.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}