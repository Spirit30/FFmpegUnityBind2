using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GigaVideoPlayer.Demo
{
    [Serializable]
    class GigaVideoPlayerFromPathDemo
    {
#if UNITY_EDITOR
        [SerializeField]
        string demoFileName = "DemoFile.mp4";
#endif

        [SerializeField]
        string videoPath = string.Empty;

        public VideoPlayerWrapper ActiveVideoPlayer { get; private set; }

        //With Validation.
        public void TryPlay()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(videoPath))
            {
                videoPath = TryFindPath();
            }
#endif

            if (!string.IsNullOrWhiteSpace(videoPath))
            {
                if (File.Exists(videoPath))
                {
                    if (ActiveVideoPlayer)
                    {
                        ActiveVideoPlayer.PlayFromPath(videoPath);
                    }
                    else
                    {
                        ActiveVideoPlayer = Video.PlayFromPath(videoPath);
                    }
                }
                else
                {
                    Debug.LogError($"File not found at: {videoPath}.");
                }
            }
            else
            {
                Debug.LogError("Please set path.");
            }
        }

#if UNITY_EDITOR
        string TryFindPath()
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                UnityEditor.AssetDatabase.GUIDToAssetPath(
                    UnityEditor.AssetDatabase.FindAssets("t:VideoClip")
                        .FirstOrDefault(
                            g => UnityEditor.AssetDatabase.GUIDToAssetPath(g)
                                .EndsWith(demoFileName))));
        }
#endif
    }
}
