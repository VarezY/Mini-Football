using System;
using System.Collections;
using DG.Tweening;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentController : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
        private static readonly int Pass = Animator.StringToHash("Pass");
        private static readonly int Tackle = Animator.StringToHash("Tackle");

        public AgentType agentType;
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

        #region Exposed Variables

        public AgentStateMachine agentStateMachine => _agentStateMachine;
        public InGameManager gameManager => _gameManager;
        public CharacterController controller => _characterController;
        public Vector3 ballPosition => _ballPosition;
        public Vector3 target => _target.transform.position;
        public Vector3 patrolPosition => _spawnPosition;
        public bool hasBall => _hasBall;

        #endregion
        
        private InGameManager _gameManager;
        private AgentStateMachine _agentStateMachine;
        private CharacterController _characterController;
        private AgentMovement _agentMovement;
        private AgentController _target;
        private AgentPassBall _passBall;
        private Tweener _reactivateTween;
        private Sequence _tackleSequence;
        private Vector3 _spawnPosition;
        private Vector3 _ballPosition;
        private float _reactiveTimeRemaining; //Debuging for Agent UI
        private bool _hasBall = false;
        // private AgentMovement _agentMovement;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _agentMovement = GetComponent<AgentMovement>();
            _passBall = GetComponent<AgentPassBall>();

            _agentStateMachine = new AgentStateMachine(this);
        }

        private void OnEnable()
        {
            _spawnPosition = transform.position;
            StartCoroutine(InitializeAgent());
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnBallCatch += BallCatch;
                InGameManager.instance.InGameEvents.OnNextMatch += KillReactivateTime;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnBallCatch -= BallCatch;
                InGameManager.instance.InGameEvents.OnNextMatch -= KillReactivateTime;
            });
        }

        private void Start()
        {
            _gameManager = InGameManager.instance;
            _tackleSequence = DOTween.Sequence();
        }

        private IEnumerator InitializeAgent()
        {
            _agentStateMachine.Initialize(_agentStateMachine.IdleState);
 
            yield return new WaitForSeconds(agentTimeToSpawn);
            switch (side)
            {
                case MatchSide.Attacker:
                    _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedNormal);
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

        public void SetBallPosition(Vector3 position)
        {
            _ballPosition = position;
        }
        
        public void ChangeColor(Color color)
        {
            shirt.material.SetColor(Color1, color);
        }

        public void MoveAgent(Vector3 position, Action callback = null)
        {
            _agentMovement.MoveToPosition(position, callback);
        }

        public void ResetAgent()
        {
            _gameManager.agentManager.activeAgents.Remove(this);
            _characterController.radius = 0f;
            _agentStateMachine.TransitionTo(_agentStateMachine.DeactivateState);
        }

        public void InactiveAgent()
        {
            _hasBall = false;
            _gameManager.agentManager.activeAgents.Remove(this);
            _agentMovement.SetMoveSpeed(0);
            _characterController.radius = 0f;
            _agentStateMachine.TransitionTo(_agentStateMachine.IdleState);
            
            AgentController a = _gameManager.agentManager.ClosestDistance(this);
            // Debug.Log(!a ? $"ASU GA ADA TMN LAGI" : $"PASS KE {a.name}");
            
            if (a)
            {
                a.WaitForBall();
                animator.SetTrigger(Pass);
                _passBall.StartPass(a);
                _reactivateTween = DOVirtual.Float(0, 1, agentAttackerReactivateTime,
                        value => _reactiveTimeRemaining = value)
                    .OnComplete(() =>
                    {
                        _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedNormal);
                        _agentStateMachine.TransitionTo(_agentStateMachine.RunState);
                    });
            }
            else
            {
                switch (agentType)
                {
                    case AgentType.Enemy:
                        _gameManager.NextMatch(AgentType.Player);
                        break;
                    case AgentType.Player:
                        _gameManager.NextMatch(AgentType.Enemy);
                        break;
                }
            }
        }

        public void ChaseAgentWithBall(AgentController targetAgent)
        {
            _target = targetAgent;
            _agentStateMachine.TransitionTo(_agentStateMachine.ChaseState);
            _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedDefender);
        }
        
        public void ReturnToPatrol()
        {
            _agentMovement.SetMoveSpeed(_agentMovement.returnSpeedDefender);
            _characterController.radius = 0f;
            defenderArea.gameObject.SetActive(false);
            animator.SetTrigger(Tackle);
        }
        
        private void WaitForBall()
        {
            _agentMovement.SetMoveSpeed(0);
        }

        private void KillReactivateTime()
        {
            _reactivateTween?.Kill();
        }
        
        private void BallCatch(AgentController agentWithBall)
        {
            if (side == MatchSide.Attacker)
                this.state = AgentState.GoToFence;
            
            if (agentWithBall != this) return;

            _characterController.center = new Vector3(0, .5f, 0);
            _characterController.radius = .25f;
            _hasBall = true;
            _agentMovement.SetMoveSpeed(_agentMovement.moveSpeedWithBall);
            ballIndicator.SetActive(true);
        }
    }
}