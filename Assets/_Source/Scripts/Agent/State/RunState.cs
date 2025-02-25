using System;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class RunState : IState
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int HasTarget = Animator.StringToHash("HasTarget");
        private AgentController _agentController;
        private Vector3 _targetFencePosition;

        public RunState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            if (_agentController.gameManager.matchManager.isBallOnPlayer)
            {
                _agentController.state = AgentState.GoToFence;
                _agentController.controller.center = new Vector3(0, 5, 0);
            }
            else
            {
                _agentController.controller.center = new Vector3(0, .5f, 0);
                _agentController.state = AgentState.SearchingBall;
            }
            
            _agentController.gameManager.agentManager.SwitchAgentStatus(_agentController);
            
            _targetFencePosition = _agentController.gameManager.matchManager.GetEnemyFencePosition(_agentController.side);
            _agentController.ChangeColor(_agentController.flagColor);
            _agentController.animator.SetFloat(Speed, 1.0f);
            _agentController.arrowIndicator.SetActive(true);
        }

        public void Update()
        {
            switch (_agentController.state)
            {
                case AgentState.SearchingBall:
                    _agentController.MoveAgent(_agentController.ballPosition);
                    break;
                case AgentState.GoToFence:
                    _agentController.MoveAgent(_targetFencePosition);
                    break;
                /*case AgentState.ChaseTarget:
                    _agentController.MoveAgent(_agentController.target);
                    break;
                case AgentState.ReturnToPatrol:
                    _agentController.MoveAgent(_agentController.patrolPosition, () =>
                    {
                        _agentController.agentStateMachine.TransitionTo(_agentController.agentStateMachine.IdleState);
                    });
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Exit()
        {
        }
    }
}
