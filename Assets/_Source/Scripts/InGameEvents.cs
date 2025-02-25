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
        
        public event Action OnTimerEnd;

        public void TimerEnd()
        {
            OnTimerEnd?.Invoke();
        }
        
        public event Action<AgentController> OnBallCatch;

        public void BallCatch(AgentController agent)
        {
            OnBallCatch?.Invoke(agent);
        }

        public event Action<AgentType> OnScoredGoal;

        public void ScoredGoal(AgentType scoredAgent)
        {
            OnScoredGoal?.Invoke(scoredAgent);
        }

        public event Action OnEndGame;

        public void EndGame()
        {
            OnEndGame?.Invoke();
        }
    }
}