using System;
using System.Collections;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentController : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_BaseColor");

        public AgentState state = AgentState.Idle;
        [Header("Agent Configuration")] 
        public MatchSide side;
        public float agentTimeToSpawn = 0.5f;
        public SphereCollider defenderArea;
        public Animator animator;
        
        [Header("Agent Color")]
        public SkinnedMeshRenderer shirt;
        public Color flagColor;
        public Color inactiveColor;
        
        [Header("Indicators")]
        public GameObject arrowIndicator;
        public GameObject ballIndicator;

        #region ExposeToStateMachine

        public CharacterController characterController => _characterController;
        public AgentStateMachine agentStateMachine => _agentStateMachine;
        public InGameManager gameManager => _gameManager;
        public Vector3 ballPosition => _ballPosition;
        public Vector3 fencePosition => _fencePosition;
        public bool hasBall => _hasBall;

        #endregion
        
        private InGameManager _gameManager;
        private AgentStateMachine _agentStateMachine;
        private CharacterController _characterController;
        private AgentMovement _agentMovement;
        private AgentController _target;
        private Vector3 _ballPosition;
        private Vector3 _fencePosition;
        private bool _hasBall = false;
        // private AgentMovement _agentMovement;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _agentMovement = GetComponent<AgentMovement>();
            
            _agentStateMachine = new AgentStateMachine(this);
        }

        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnBallCatch += BallCatch;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnBallCatch -= BallCatch;
            });
        }

        private IEnumerator Start()
        {
            _gameManager = InGameManager.instance;
            _ballPosition = _gameManager.matchManager.GetBallPosition();
            _fencePosition = _gameManager.matchManager.GetFencePosition(side);
            ChangeColor(inactiveColor);
            _agentStateMachine.Initialize(_agentStateMachine.IdleState);

            yield return new WaitForSeconds(agentTimeToSpawn);
            switch (side)
            {
                case MatchSide.Attacker:
                    _agentStateMachine.TransitionTo(_agentStateMachine.RunState);
                    break;
                case MatchSide.Defender:
                    _agentStateMachine.TransitionTo(_agentStateMachine.PatrolState);
                    break;
            }
        }

        private void Update()
        {
            _agentStateMachine.Update();
        }

        public void ChangeColor(Color color)
        {
            shirt.material.SetColor(Color1, color);
        }

        public void MoveAgent(Vector3 position)
        {
            _agentMovement.MoveToPosition(position);
        }

        public void ChaseAgentWithBall(AgentController target)
        {
            _target = target;
            state = AgentState.ChaseTarget;
            _agentStateMachine.TransitionTo(_agentStateMachine.RunState);
        }
        
        private void BallCatch(AgentController agentWithBall)
        {
            this.state = AgentState.GoToFence;
            if (agentWithBall != this) return;
            
            _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedWithBall);
            _characterController.radius = 0.25f;
            ballIndicator.SetActive(true);
        }
    }
}