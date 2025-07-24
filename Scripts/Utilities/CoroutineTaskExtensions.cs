using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace EasyAssets.InternetChecker.Scripts.Utilities
{
    public static class CoroutineTaskExtensions
    {
        public static IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
        }

        public static IEnumerator WaitForTask<TResult>(Task<TResult> task, Action<TResult> onComplete)
        {
            while (!task.IsCompleted) yield return null;

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                onComplete?.Invoke(default);
            }
            else
            {
                onComplete?.Invoke(task.Result);
            }
        }
    }
}