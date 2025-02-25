using System;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class ChaseState : IState
    {
        private static readonly int HasTarget = Animator.StringToHash("HasTarget");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private AgentController _agentController;
        private Vector3 _targetFencePosition;

        public ChaseState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            _agentController.state = AgentState.ChaseTarget;
            _agentController.animator.SetBool(HasTarget, true);
            _agentController.defenderIndicator.SetActive(false);
            _agentController.animator.SetFloat(Speed, 1.0f);
            _agentController.ChangeColor(_agentController.flagColor);
        }

        public void Update()
        {
            switch (_agentController.state)
            {
                case AgentState.ChaseTarget:
                    _agentController.MoveAgent(_agentController.target);
                    break;
                case AgentState.ReturnToPatrol:
                    _agentController.MoveAgent(_agentController.patrolPosition, () =>
                    {
                        _agentController.agentStateMachine.TransitionTo(_agentController.agentStateMachine.IdleState);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Exit()
        {
        }
    }
}