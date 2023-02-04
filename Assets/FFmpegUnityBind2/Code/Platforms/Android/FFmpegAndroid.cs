using FFmpegUnityBind2.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace FFmpegUnityBind2.Android
{
    public static class FFmpegAndroid
    {
        public const string PACKAGE_NAME = "com.ffmpeg.unity.bind2";
        const string CLASS_NAME = PACKAGE_NAME + ".Bridge";
        const string EXECUTE_METHOD_NAME = "execute";
        const string CANCEL_METHOD_NAME = "cancel";

        static AndroidJavaClass @class;
        static AndroidJavaClass Class
        {
            get
            {
                if (@class == null)
                {
                    @class = new AndroidJavaClass(CLASS_NAME);
                }
                return @class;
            }
        }

        public static long Execute(string command, List<IFFmpegCallbacksHandler> callbacksHandlers)
        {
            //Initialize Mobile Callbacks Handler to start listen immediately
            var mobileCallbacksHandler = FFmpegMobileCallbacksHandler.Instance;
            long executionId = Class.CallStatic<long>(EXECUTE_METHOD_NAME, command);
            mobileCallbacksHandler.RegisterCallbackHandlers(executionId, callbacksHandlers);
            return executionId;
        }

        public static void Cancel(long executionId)
        {
            Class.CallStatic(CANCEL_METHOD_NAME, executionId);
        }
    }
}