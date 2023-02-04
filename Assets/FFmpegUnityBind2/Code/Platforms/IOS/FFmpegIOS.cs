using FFmpegUnityBind2.Shared;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFmpegUnityBind2.IOS
{
    public static class FFmpegIOS
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern long execute(string command);

        [DllImport("__Internal")]
        static extern void cancel(long executionId);
#endif

        public static long Execute(string command, List<IFFmpegCallbacksHandler> callbacksHandlers)
        {
#if UNITY_IOS
            //Initialize Mobile Callbacks Handler to start listen immediately
            var mobileCallbacksHandler = FFmpegMobileCallbacksHandler.Instance;
            long executionId = execute(command);
            mobileCallbacksHandler.RegisterCallbackHandlers(executionId, callbacksHandlers);
            return executionId;
#else
            throw FFmpeg.PlatformNotSupportedException();
#endif
        }

        public static void Cancel(long executionId)
        {
#if UNITY_IOS
            cancel(executionId);
#else
            throw FFmpeg.PlatformNotSupportedException();
#endif
        }
    }
}