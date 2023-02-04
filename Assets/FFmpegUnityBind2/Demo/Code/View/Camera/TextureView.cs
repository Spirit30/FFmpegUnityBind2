using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class TextureView : MonoBehaviour
    {
        [SerializeField]
        RawImage image = null;

        [SerializeField]
        AspectRatioFitter aspectRatioFitter = null;

        public void Open(Texture texture)
        {
            image.texture = texture;
            aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}