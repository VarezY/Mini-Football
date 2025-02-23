using UnityEngine;

namespace MiniFootball.Agent
{
    public class IdleState : IState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private AgentController _agentController;

        public IdleState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            _agentController.animator.SetTrigger(Idle);
            _agentController.ChangeColor(_agentController.inactiveColor);
            _agentController.characterController.radius = 0f;
            _agentController.arrowIndicator.SetActive(false);
        }

        public void Update()
        {
            
        }

        public void Exit()
        {
        }
    }
}