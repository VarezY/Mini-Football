using System;

namespace MiniFootball
{
    public struct InGameEvents
    {
        public event Action OnStartGame;
        
        public Action<int> OnSpawnAgent;

        public void StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}