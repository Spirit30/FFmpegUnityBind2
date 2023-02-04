using System;
using System.Collections.Generic;
using UnityEngine;

namespace FFmpegUnityBind2.Shared
{
    using Internal;

    public class FFmpegMobileCallbacksHandler : MonoBehaviour
    {
        static FFmpegMobileCallbacksHandler instance;
        public static FFmpegMobileCallbacksHandler Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject(typeof(FFmpegMobileCallbacksHandler).Name).AddComponent<FFmpegMobileCallbacksHandler>();
                }
                return instance;
            }
        }

        //<ExecutionId, Callbacks Handler>
        readonly static Dictionary<long, List<IFFmpegCallbacksHandler>> allCallbacksHandlers = new Dictionary<long, List<IFFmpegCallbacksHandler>>();
        //<ExecutionId, Messages>
        readonly static Dictionary<long, Queue<string>> allUnhandledMessages = new Dictionary<long, Queue<string>>();

        public static bool ReleaseHandlersAfterExecutionEnd { get; set; }

        public void RegisterCallbackHandlers(long executionId, List<IFFmpegCallbacksHandler> callbacksHandlers)
        {
            allCallbacksHandlers.Add(executionId, callbacksHandlers);

            if(allUnhandledMessages.ContainsKey(executionId))
            {
                var unhandledMessages = allUnhandledMessages[executionId];

                while(unhandledMessages.Count > 0)
                {
                    OnFFmpegMobileCallback(unhandledMessages.Dequeue());
                }

                allUnhandledMessages.Remove(executionId);
            }
        }

        //Native Interface
        public void OnFFmpegMobileCallback(string eventText)
        {
            //For more intensive debug of native part
            //if (FFmpeg.IsPrintingLog)
            //{
            //    Debug.Log("Early: " + eventText);
            //}

            var eventProperties = eventText.Split('|');

            if (eventProperties.Length >= 2)
            {
                string eventType = eventProperties[0];
                long executionId = long.Parse(eventProperties[1]);
                string message = eventProperties.Length >= 3 ? eventProperties[2] : null;

                //Handle Message.
                if(allCallbacksHandlers.ContainsKey(executionId))
                {
                    var callbacksHandlers = allCallbacksHandlers[executionId];

                    switch (eventType)
                    {
                        case "OnStart":

                            for (int i = 0; i < callbacksHandlers.Count; i++)
                            {
                                var @event = new Event(callbacksHandlers[i], executionId, EventType.OnStart);
                                EventDispatcher.Invoke(@event);
                            }
                            break;

                        case "OnLog":

                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                for (int i = 0; i < callbacksHandlers.Count; i++)
                                {
                                    var @event = new Event(callbacksHandlers[i], executionId, EventType.OnLog, message);
                                    EventDispatcher.Invoke(@event);
                                }
                            }
                            break;

                        case "OnWarning":

                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                for (int i = 0; i < callbacksHandlers.Count; i++)
                                {
                                    var @event = new Event(callbacksHandlers[i], executionId, EventType.OnWarning, message);
                                    EventDispatcher.Invoke(@event);
                                }
                            }
                            break;

                        case "OnError":

                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                for (int i = 0; i < callbacksHandlers.Count; i++)
                                {
                                    var @event = new Event(callbacksHandlers[i], executionId, EventType.OnError, message);
                                    EventDispatcher.Invoke(@event);
                                }
                            }
                            break;

                        case "OnSuccess":

                            for (int i = 0; i < callbacksHandlers.Count; i++)
                            {
                                var @event = new Event(callbacksHandlers[i], executionId, EventType.OnSuccess);
                                EventDispatcher.Invoke(@event);
                            }

                            if (ReleaseHandlersAfterExecutionEnd)
                            {
                                RemoveCallbackHandlers(executionId);
                            }

                            break;

                        case "OnCanceled":

                            for (int i = 0; i < callbacksHandlers.Count; i++)
                            {
                                var @event = new Event(callbacksHandlers[i], executionId, EventType.OnCanceled);
                                EventDispatcher.Invoke(@event);
                            }

                            if (ReleaseHandlersAfterExecutionEnd)
                            {
                                RemoveCallbackHandlers(executionId);
                            }

                            break;

                        case "OnFail":

                            for (int i = 0; i < callbacksHandlers.Count; i++)
                            {
                                var @event = new Event(callbacksHandlers[i], executionId, EventType.OnFail);
                                EventDispatcher.Invoke(@event);
                            }

                            if (ReleaseHandlersAfterExecutionEnd)
                            {
                                RemoveCallbackHandlers(executionId);
                            }

                            break;

                        default:

                            throw new Exception($"Unknown {Application.platform} event type {eventType}.");
                    }
                }
                //Queue for later handling.
                else
                {
                    if(allUnhandledMessages.ContainsKey(executionId))
                    {
                        allUnhandledMessages[executionId].Enqueue(eventText);
                    }
                    else
                    {
                        var messagesQueue = new Queue<string>();
                        messagesQueue.Enqueue(eventText);
                        allUnhandledMessages.Add(executionId, messagesQueue);
                    }
                }
            }
        }

        void RemoveCallbackHandlers(long executionId)
        {
            allCallbacksHandlers.Remove(executionId);
        }
    }
}