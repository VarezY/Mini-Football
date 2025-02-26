using UnityEngine;

namespace MiniFootball.Agent
{
    public class DeactivateState : IState
    {
        private static readonly int Celebrate = Animator.StringToHash("Celebrate");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private AgentController _agentController;

        public DeactivateState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            _agentController.state = AgentState.InFence;
            _agentController.animator.SetFloat(Speed, 0f);
            _agentController.animator.SetTrigger(Celebrate);
            
            _agentController.ChangeColor(_agentController.inactiveColor);
            _agentController.controller.center = new Vector3(0, .5f, 0);
 
            _agentController.arrowIndicator.SetActive(false);
            _agentController.ballIndicator.SetActive(false);
            _agentController.defenderIndicator.SetActive(false);
            _agentController.defenderArea.gameObject.SetActive(false);

        }

        public void Update()
        {
            if (_agentController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _agentController.agentStateMachine.TransitionTo(_agentController.agentStateMachine.IdleState);
            }
        }

        public void Exit()
        {
        }
    }
}