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
            _agentController.arrowIndicator.SetActive(false);
        }

        public void Update()
        {
            if (_agentController.status == AgentController.AgentStatus.Active)
            {
                _agentController.agentStateMachine.TransitionTo(_agentController.agentStateMachine.RunState);
            }
        }

        public void Exit()
        {
        }
    }
}