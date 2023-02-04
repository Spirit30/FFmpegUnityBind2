using UnityEngine;

namespace FFmpegUnityBind2.Demo
{
    class ConsoleEventsHandler : IFFmpegCallbacksHandler
    {
        public void OnStart(long executionId)
        {
            Console.WriteLine($"OnStart. Execution Id: {executionId}");
        }

        public void OnLog(long executionId, string message)
        {
            Console.WriteLine($"OnLog. Execution Id: {executionId}. Message: {message}");
        }

        public void OnWarning(long executionId, string message)
        {
            Console.WriteWarning($"OnWarning. Execution Id: {executionId}. Message: {message}");
        }

        public void OnError(long executionId, string message)
        {
            Console.WriteError($"OnError. Execution Id: {executionId}. Message: {message}");
        }

        public void OnSuccess(long executionId)
        {
            Console.WriteLine($"OnSuccess. Execution Id: {executionId}");
        }

        public void OnCanceled(long executionId)
        {
            Console.WriteLine($"OnCanceled. Execution Id: {executionId}");
        }

        public void OnFail(long executionId)
        {
            Console.WriteError($"OnFail. Execution Id: {executionId}");
        }
    }
}