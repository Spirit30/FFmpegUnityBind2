using FFmpegUnityBind2.Components;
using GigaVideoPlayer;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCaseSharedView : MonoBehaviour
    {
        [SerializeField]
        Text nameLable = null;

        [SerializeField]
        Text descriptionLable = null;

        [SerializeField]
        Button closeButton = null;

        [SerializeField]
        DemoCasesView demoCasesView = null;

        [SerializeField]
        FFmpegREC ffmpegREC = null;

        public FFmpegREC FFmpegREC => ffmpegREC;

        [SerializeField]
        SpinningWheelView spinningWheelView = null;

        [SerializeField]
        AudioSource audioSource = null;

        [SerializeField]
        CanvasGroup[] canvasGroups = null;

        [SerializeField]
        ColorSheme videoPlayerColorSheme = new ColorSheme
        {
            colorA = Color.green
        };

        VideoPlayerWrapper activeVideoPlayer;

        public void Open(string name, string description)
        {
            nameLable.text = name;
            descriptionLable.text = description;

            demoCasesView.CloseAllDemoCases();
            demoCasesView.Close();

            gameObject.SetActive(true);
        }

        public void SetupSceneForStartCapturing(bool audio)
        {
            spinningWheelView.Open();
            SetCanvasGroupsAlpha(0.5f);

            if (audio)
            {
                audioSource.Play();
            }
        }

        public void SetupSceneForStopCapturing(bool audio)
        {
            spinningWheelView.Close();
            SetCanvasGroupsAlpha(1.0f);

            if (audio)
            {
                audioSource.Stop();
            }
        }

        public void SetupSceneForCancelCapturing()
        {
            spinningWheelView.Close();
            SetCanvasGroupsAlpha(1.0f);

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            Console.Clear();
        }

        public void SetCanvasGroupsAlpha(float alpha)
        {
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = alpha;
            }
        }

        public void OnSuccess(long executionId, string outputPath)
        {
            if (File.Exists(outputPath))
            {
                const string SEPARATOR = "------------------------------";
                Console.WriteLine(SEPARATOR);
                Console.WriteLine($"RESULT ExecutionId: {executionId}. Output: {outputPath}");
                Console.WriteLine(SEPARATOR);

                if (ChooseVideoFileView.IsVideo(outputPath))
                {
                    PlayVideo(outputPath);
                }
            }
        }

        void PlayVideo(string path)
        {
            if (activeVideoPlayer)
            {
                activeVideoPlayer.PlayFromPath(path);
            }
            else
            {
                activeVideoPlayer = Video.PlayFromPath(path);
            }

            SetupActiveVideoPlayer();
        }

        void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButton);
        }

        void Update()
        {
            if (activeVideoPlayer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    activeVideoPlayer.Play(!activeVideoPlayer.IsPlay);
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    activeVideoPlayer.Stop();
                }
            }
        }

        void SetupActiveVideoPlayer()
        {
            if (activeVideoPlayer)
            {
                activeVideoPlayer.IsLoop = false;
                activeVideoPlayer.Theme.SetScheme(videoPlayerColorSheme);
            }
        }

        void OnCloseButton()
        {
            demoCasesView.Open();
            Close();
        }

        void Close()
        {
            gameObject.SetActive(false);
        }
    }
}