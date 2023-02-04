using UnityEngine;
using UnityEngine.UI;

namespace GigaVideoPlayer.Internal
{
    class VideoCanvas : MonoBehaviour
    {
        [SerializeField]
        RawImage image = null;

        [SerializeField]
        AspectRatioFitter aspectRatioFitter = null;

        [SerializeField]
        Theme theme = null;

        bool hasTexture;

        void Update()
        {
            bool hasTexture = theme.VideoPlayer.Texture;

            if (this.hasTexture != hasTexture)
            {
                this.hasTexture = hasTexture;

                UpdateTextureState();
            }
        }

        void UpdateTextureState()
        {
            if (hasTexture)
            {
                var texture  = theme.VideoPlayer.Texture;
                image.texture = texture;
                image.enabled = true;
                aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            }
            else
            {
                image.enabled = false;
            }
        }
    }
}