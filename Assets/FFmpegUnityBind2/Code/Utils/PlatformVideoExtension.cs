using UnityEngine;

namespace FFmpegUnityBind2.Utils
{
    static class PlatformVideoExtension
    {
        public static string GetCurrent()
        {
            switch(Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer: return ".wmv";
                case RuntimePlatform.IPhonePlayer: return ".mov";
                default: return ".mp4";
            }
        }
    }
}