using System;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class RunState : IState
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private AgentController _agentController;

        public RunState(AgentController agentController)
        {
            _agentController = agentController;
        }
        
        public void Enter()
        {
            _agentController.state = _agentController.gameManager.matchManager.isBallOnPlayer? AgentState.GoToFence : AgentState.SearchingBall;
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
                    _agentController.MoveAgent(_agentController.fencePosition);
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
