using FFmpegUnityBind2.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpegUnityBind2.Demo
{
    class DemoCasesView : MonoBehaviour
    {
        [SerializeField, TextArea]
        string description = null;

        [SerializeField]
        Button[] demoCasesButtons = null;

        [SerializeField]
        DemoCaseView[] demoCases = null;

        [SerializeField]
        SpinningWheelView spinningWheelView = null;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void CloseAllDemoCases()
        {
            foreach (var demoCase in demoCases)
            {
                demoCase.Close();
            }
        }

        void OnEnable()
        {
            Console.Clear();
            Console.WriteLine(description);
        }

        void Awake()
        {
            for(int i = 0; i < demoCasesButtons.Length; ++i)
            {
                //Bind buttons to Demo Cases
                demoCasesButtons[i].onClick.AddListener(demoCases[i].Open);
            }
        }

        IEnumerator Start()
        {
            spinningWheelView.Open();

            string demoRelativeFilePath = "Demo/DemoFile.mp4";
            string demoFilePath = Path.Combine(Application.persistentDataPath, demoRelativeFilePath);

            if (!File.Exists(demoFilePath))
            {
                yield return FileUnpacker.UnpackFileOperation(demoRelativeFilePath, demoFilePath);
                Debug.Log($"Unpacked to: {Application.persistentDataPath}");
            }

            spinningWheelView.Close();
        }
    }
}