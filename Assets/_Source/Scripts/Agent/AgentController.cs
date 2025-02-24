using System;
using System.Collections;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentController : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
        private static readonly int HasTarget = Animator.StringToHash("HasTarget");

        public AgentState state = AgentState.Idle;
        
        [Header("Agent Configuration")] 
        public MatchSide side;
        public Animator animator;
        public SphereCollider defenderArea;
        public float agentTimeToSpawn = 0.5f;
        public float agentAttackerReactivateTime = 2.5f;
        public float agentDefenderReactivateTime = 4f;
        
        [Header("Agent Color")]
        public SkinnedMeshRenderer shirt;
        public Color flagColor;
        public Color inactiveColor;
        
        [Header("Indicators")]
        public GameObject arrowIndicator;
        public GameObject ballIndicator;
        public GameObject defenderIndicator;

        #region ExposeToStateMachine

        public AgentStateMachine agentStateMachine => _agentStateMachine;
        public InGameManager gameManager => _gameManager;
        public Vector3 ballPosition => _ballPosition;
        public Vector3 fencePosition => _fencePosition;
        public Vector3 target => _target.transform.position;
        public Vector3 patrolPosition => _spawnPosition;
        public bool hasBall => _hasBall;

        #endregion
        
        private InGameManager _gameManager;
        private AgentStateMachine _agentStateMachine;
        private CharacterController _characterController;
        private AgentMovement _agentMovement;
        private AgentController _target;
        private Vector3 _spawnPosition;
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
            _spawnPosition = transform.position;
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

        public void MoveAgent(Vector3 position, Action callback = null)
        {
            _agentMovement.MoveToPosition(position, callback);
        }

        public void InactiveAgent()
        {
            _agentMovement.SetMoveSpeed(0);
            _characterController.radius = 0f;
            _agentStateMachine.TransitionTo(_agentStateMachine.IdleState);
        }

        public void ReturnToPatrol()
        {
            _agentMovement.SetMoveSpeed(_agentMovement.returnSpeedDefender);
            _characterController.radius = 0f;
            state = AgentState.ReturnToPatrol;
        }

        public void ChaseAgentWithBall(AgentController targetAgent)
        {
            _target = targetAgent;
            _agentStateMachine.TransitionTo(_agentStateMachine.RunState);
            _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedDefender);
            animator.SetBool(HasTarget, true);
            _characterController.radius = .25f;
            state = AgentState.ChaseTarget;
        }
        
        private void BallCatch(AgentController agentWithBall)
        {
            this.state = AgentState.GoToFence;
            if (agentWithBall != this) return;

            _characterController.radius = .275f;
            _hasBall = true;
            _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedWithBall);
            ballIndicator.SetActive(true);
        }
    }
}