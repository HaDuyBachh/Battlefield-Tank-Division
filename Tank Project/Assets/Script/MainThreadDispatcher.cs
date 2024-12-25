using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    public static void Enqueue(Action action)
    {
        _executionQueue.Enqueue(action);
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }
}
