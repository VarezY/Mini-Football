using UnityEngine;

namespace MiniFootball.Agent
{
    public class PatrolState : IState
    {
        private static readonly int Patrol = Animator.StringToHash("Patrol");
        private static readonly int HasTarget = Animator.StringToHash("HasTarget");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private AgentController _agentController;
        
        public PatrolState(AgentController agent)
        {
            _agentController = agent;
        }
        
        public void Enter()
        {
            _agentController.ChangeColor(_agentController.flagColor);
            _agentController.animator.SetTrigger(Patrol);
            _agentController.animator.SetBool(HasTarget, false);
            _agentController.defenderIndicator.SetActive(true);
            _agentController.animator.SetFloat(Speed, 0f);
            _agentController.defenderArea.gameObject.SetActive(true);
            _agentController.controller.center = new Vector3(0, .5f, 0);
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }
    }
}