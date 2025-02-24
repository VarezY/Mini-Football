using UnityEngine;

namespace MiniFootball.Agent
{
    public class PatrolState : IState
    {
        private static readonly int Patrol = Animator.StringToHash("Patrol");
        private AgentController _agentController;
        
        public PatrolState(AgentController agent)
        {
            _agentController = agent;
        }
        
        public void Enter()
        {
            _agentController.ChangeColor(_agentController.flagColor);
            _agentController.animator.SetTrigger(Patrol);
            _agentController.defenderIndicator.SetActive(true);
            _agentController.defenderArea.gameObject.SetActive(true);
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }
    }
}