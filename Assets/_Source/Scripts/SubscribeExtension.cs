using System;
using System.Collections;
using UnityEngine;

namespace MiniFootball
{
    public static class SubscribeExtension
    {
        public static IEnumerator WaitAndSubscribe(this MonoBehaviour mono, Action callback)
        {
            while (!InGameManager.instance)
                yield return null;
            
            callback?.Invoke();  // Add null check for safety
        }

        public static void WaitAndUnSubscribe(this MonoBehaviour mono, Action callback)
        {
            if (InGameManager.instance)
            {
                callback?.Invoke();
            }
        }
    }
}