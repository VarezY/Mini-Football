using UnityEngine;

namespace MiniFootball.Agent
{
    public class IdleState : IState
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int HasTarget = Animator.StringToHash("HasTarget");
        private AgentController _agentController;

        public IdleState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            _agentController.state = AgentState.Idle;
            _agentController.animator.SetTrigger(Idle);
            _agentController.animator.SetFloat(Speed, 0f);
            _agentController.animator.SetBool(HasTarget, false);
            _agentController.ChangeColor(_agentController.inactiveColor);
            _agentController.arrowIndicator.SetActive(false);
            _agentController.ballIndicator.SetActive(false);
            _agentController.defenderIndicator.SetActive(false);
            _agentController.defenderArea.gameObject.SetActive(false);
        }

        public void Update()
        {
            
        }

        public void Exit()
        {
        }
    }
}