using System;
using MiniFootball.Agent;

namespace MiniFootball
{
    public struct InGameEvents
    {
        public event Action OnStartGame;
        public void StartGame()
        {
            OnStartGame?.Invoke();
        }
        
        public event Action OnNextMatch;
        public void NextMatch()
        {
            OnNextMatch?.Invoke();
        }
        
        public event Action<AgentController> OnBallCatch;

        public void BallCatch(AgentController agent)
        {
            OnBallCatch?.Invoke(agent);
        }

        public event Action<AgentController> OnTriggerDefender;

        // public event Action OnEndGame;
    }
}