using System;
using System.Linq;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class CameraView : MonoBehaviour
    {
        [SerializeField]
        RawImage image = null;

        [SerializeField]
        AspectRatioFitter aspectRatioFitter = null;

        [SerializeField]
        bool isFrontFacing = false;

        WebCamTexture webCamTexture;

        public Texture Texture => webCamTexture;

        public void Open()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ValidateAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
            ValidateIOS();
#endif
            var devices = WebCamTexture.devices;

            if (devices.Length <= 0)
            {
                throw new Exception("Any Camera Device is not found.");
            }

            webCamTexture = new WebCamTexture();
            var device = devices.Any(d => d.isFrontFacing == isFrontFacing) ? devices.First(d => d.isFrontFacing == isFrontFacing) : devices.First();
            webCamTexture.deviceName = device.name;
            image.texture = webCamTexture;
            webCamTexture.Play();

            aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;

            Show(true);
        }

        public void Close()
        {
            if (webCamTexture)
            {
                webCamTexture.Stop();
                webCamTexture = null;
            }

            Show(false);
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        void ValidateAndroid()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                throw new Exception("Please give Camera permission to capture.");
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
                throw new Exception("Please give Microphone permission to record.");
            }
        }

#endif

#if UNITY_IOS && !UNITY_EDITOR
        void ValidateIOS()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
                throw new Exception("Please give Camera permission to capture.");
            }

            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
                throw new Exception("Please give Microphone permission to record.");
            }
        }
#endif

        void Show(bool flag)
        {
            image.gameObject.SetActive(flag);
            gameObject.SetActive(flag);
        }
    }
}