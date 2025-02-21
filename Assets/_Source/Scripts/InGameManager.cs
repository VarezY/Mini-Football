using System;
using UnityEngine;

namespace MiniFootball
{
    public class InGameManager : MonoBehaviour
    {
        public static InGameManager instance { get; private set; }

        public UIManager uiManager { get; private set; }
        
        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;

            uiManager.GetComponentInChildren<UIManager>();
        }
    }
}
