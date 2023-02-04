using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace FFmpegUnityBind2.Utils
{
    public class FileUnpacker : MonoBehaviour
    {
        static FileUnpacker instance;
        static FileUnpacker Instance
        {
            get
            {
                if(!instance)
                {
                    instance = new GameObject(nameof(FileUnpacker)).AddComponent<FileUnpacker>();
                }
                return instance;
            }
        }

        public static void UnpackFile(string relativePath, string destinationPath)
        {
            Instance.StartCoroutine(UnpackFileOperation(relativePath, destinationPath));
        }

        public static void UnpackFiles(string[] relativePaths, string[] destinationPaths)
        {
            Instance.StartCoroutine(UnpackFilesOperation(relativePaths, destinationPaths));
        }

        public static IEnumerator UnpackFileOperation(string relativePath, string mainDestinationPath)
        {
            Debug.Log($"relativePath: {relativePath}");
            Debug.Log($"destinationPath: {mainDestinationPath}");

            relativePath = ToZipPath(relativePath);
            string streamingAssetsSourcePath = Path.Combine(Application.streamingAssetsPath, relativePath);
            Debug.Log($"streamingAssetsSourcePath: {streamingAssetsSourcePath}");
            string streamingAssetsSourceUri = Application.platform == RuntimePlatform.Android
                ? streamingAssetsSourcePath
                : "file://" + streamingAssetsSourcePath;
            Debug.Log($"streamingAssetsSourceUri: {streamingAssetsSourceUri}");

            #pragma warning disable 0618
            var streamingAssetsOperation = new WWW(streamingAssetsSourceUri);
            #pragma warning restore 0618
            yield return streamingAssetsOperation;

            byte[] bytes = new byte[streamingAssetsOperation.bytesDownloaded];
            Array.Copy(streamingAssetsOperation.bytes, bytes, streamingAssetsOperation.bytesDownloaded);

            string destinationDirectory = Path.GetDirectoryName(mainDestinationPath);
            Directory.CreateDirectory(destinationDirectory);
            Debug.Log($"destinationDirectory: {destinationDirectory}");

            string destinationZipPath = ToZipPath(mainDestinationPath);
            File.WriteAllBytes(destinationZipPath, bytes);
            Debug.Log($"destinationZipPath: {destinationZipPath}");

            string tempDestinationDirectory = Path.Combine(destinationDirectory, $"TempUnzipDirectory");
            Directory.CreateDirectory(tempDestinationDirectory);
            Debug.Log($"tempDestinationDirectory: {tempDestinationDirectory}");

            new FastZip().ExtractZip(destinationZipPath, tempDestinationDirectory, null);

            var tempDestinationPaths = new List<string>(Directory.GetFiles(tempDestinationDirectory));

            string mainTempDestinationPath = Path.Combine(tempDestinationDirectory, Path.GetFileName(mainDestinationPath));
            tempDestinationPaths.Remove(mainTempDestinationPath);
            tempDestinationPaths.Add(mainTempDestinationPath);

            Debug.Log($"mainTempDestinationPath: {tempDestinationPaths.Last()}");

            foreach (string tempDestinationPath in tempDestinationPaths)
            {
                Debug.Log($"tempDestinationPath: {tempDestinationPath}");

                string destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(tempDestinationPath));

                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                File.Move(tempDestinationPath, destinationPath);
            }

            File.Delete(destinationZipPath);
            Directory.Delete(tempDestinationDirectory, true);
        }

        static IEnumerator UnpackFilesOperation(string[] relativePaths, string[] destinationPaths)
        {
            if(relativePaths.Length != destinationPaths.Length)
            {
                throw new ArgumentException("relativePaths.Length should be equal destinationPaths.Length");
            }

            for(int i = 0; i < relativePaths.Length; ++i)
            {
                yield return UnpackFileOperation(relativePaths[i], destinationPaths[i]);
            }
        }

        static string ToZipPath(string path)
        {
            return Path.ChangeExtension(path, ".zip");
        }
    }
}