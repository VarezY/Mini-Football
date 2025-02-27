using System;
using MiniFootball.Agent;

namespace MiniFootball
{
    public struct InGameEvents
    {
        #region UI Events

        public event Action<AgentType, float> OnEnergyChanged;

        public void EnergyChanged(AgentType type, float energy)
        {
            OnEnergyChanged?.Invoke(type, energy);
        }

        #endregion

        #region Game Events

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

        public event Action OnEndGame;

        public void EndGame()
        {
            OnEndGame?.Invoke();
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

        #endregion

        #region AR Events

        public event Action<bool> OnSwitchAR;

        public void SwitchAR(bool isSwitched)
        {
            OnSwitchAR?.Invoke(isSwitched);
        }

        #endregion
    }
}