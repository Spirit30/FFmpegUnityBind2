using System;
using System.Collections.Generic;
using UnityEngine;

class Dispatcher : MonoBehaviour
{
    static Dispatcher instance;
    static Dispatcher Instance
    {
        get
        {
            if (!instance)
            {
                throw new MissingReferenceException("Please add \"Dispatcher\" prefab to the scene.");
            }
            return instance;
        }
    }

    Queue<Action> actions = new Queue<Action>();

    void Awake()
    {
        if (instance && instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        if(actions.Count > 0)
        {
            Action action = actions.Dequeue();
            action?.Invoke();
        }
    }

    public static void Invoke(Action action)
    {
        Instance.actions.Enqueue(action);
    }
}