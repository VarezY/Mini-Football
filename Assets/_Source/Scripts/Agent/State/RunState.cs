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

        public RunState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            switch (_agentController.side)
            {
                case MatchSide.Attacker:
                    _agentController.state = _agentController.gameManager.matchManager.isBallOnPlayer? AgentState.GoToFence : AgentState.SearchingBall;
                    _agentController.gameManager.agentManager.SwitchAgentStatus(_agentController);
                    break;
                case MatchSide.Defender:
                    _agentController.state = AgentState.ChaseTarget;
                    _agentController.animator.SetBool(HasTarget, true);
                    _agentController.defenderIndicator.SetActive(false);
                    break;
            }

            _agentController.ChangeColor(_agentController.flagColor);
            _agentController.animator.SetFloat(Speed, 1.0f);
            _agentController.arrowIndicator.SetActive(true);
            _agentController.controller.radius = .25f;
        }

        public void Update()
        {
            switch (_agentController.state)
            {
                case AgentState.SearchingBall:
                    _agentController.MoveAgent(_agentController.ballPosition);
                    break;
                case AgentState.GoToFence:
                    _agentController.MoveAgent(_agentController.fencePosition);
                    break;
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
