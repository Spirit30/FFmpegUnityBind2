using UnityEngine;

namespace FFmpegUnityBind2
{
    public class FFmpegCallbacksHandlerDebug : IFFmpegCallbacksHandler
    {
        public void OnStart(long executionId)
        {
            Debug.Log($"On Start. Execution Id: {executionId}");
        }

        public void OnLog(long executionId, string message)
        {
            Debug.Log($"On Log. Execution Id: {executionId}. Message: {message}");
        }

        public void OnWarning(long executionId, string message)
        {
            Debug.LogWarning($"On Warning. Execution Id: {executionId}. Message: {message}");
        }

        public void OnError(long executionId, string message)
        {
            Debug.LogError($"On Error. Execution Id: {executionId}. Message: {message}");
        }

        public void OnSuccess(long executionId)
        {
            Debug.Log($"On Success. Execution Id: {executionId}");
        }

        public void OnCanceled(long executionId)
        {
            Debug.Log($"On Canceled. Execution Id: {executionId}");
        }

        public void OnFail(long executionId)
        {
            Debug.LogError($"On Fail. Execution Id: {executionId}");
        }
    }
}