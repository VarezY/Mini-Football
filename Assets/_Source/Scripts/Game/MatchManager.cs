using System;
using System.Collections;
using MiniFootball.Agent;
using MiniFootball.UI;
using MiniFootball.UI.NewEnergyBar;
using UnityEngine;

namespace MiniFootball.Game
{
    public class MatchManager : MonoBehaviour
    {
        public enum GameState
        {
            Starting,
            Playing,
            Scoring
        }

        [Header("Debug Info (Read Only)")]
        public int currentMatch = 1;
        public int playerScore = 0;
        public int enemyScore = 0;

        [SerializeField] private GameState currentState = GameState.Starting;
        public MatchSide playerStatus = MatchSide.Attacker;
        public MatchSide enemyStatus = MatchSide.Defender;
        public bool isBallOnPlayer;

        [Header("Match Configuration")] 
        [SerializeField] private GameObject ball;
        public Transform stadiumGround;
        public int maxMatches = 6;
        public int timer = 140;
        public int timeToFillEnergy = 2;

        [Header("Player Configuration")]
        [SerializeField] private BoxCollider spawnPlayerArea;
        [SerializeField] private BoxCollider fencePlayer;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private EnergySystem playerEnergySystem;

        [Header("Enemy Configuration")] 
        [SerializeField] private BoxCollider spawnEnemyArea;
        [SerializeField] private BoxCollider fenceEnemy;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private EnergySystem enemyEnergySystem;

        private Coroutine _rechargePlayerCoroutine = null;
        private Coroutine _rechargeEnemyCoroutine = null;
        
        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch += NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal += UpdatePlayerScore;
                InGameManager.instance.InGameEvents.OnBallCatch += OnCatch;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame -= StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch -= NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal -= UpdatePlayerScore;
                InGameManager.instance.InGameEvents.OnBallCatch -= OnCatch;
            });
        }

        private void Start()
        {
            playerEnergySystem.Initialize(this);
            enemyEnergySystem.Initialize(this);
        }

        private void OnDestroy()
        {
            playerEnergySystem.Cleanup();
            enemyEnergySystem.Cleanup();        }

        public bool CanSpawn(AgentType type, int requiredEnergy)
        {
            switch (type)
            {
                case AgentType.Player when requiredEnergy <= playerEnergySystem.CurrentEnergy:
                    DecreaseEnergy(type, requiredEnergy);
                    return true;
                case AgentType.Enemy when requiredEnergy <= enemyEnergySystem.CurrentEnergy:
                    DecreaseEnergy(type, requiredEnergy);
                    return true;
                default:
                    return false;
            }
        }

        private void IncreaseEnergy(AgentType type, float amount)
        {
            switch (type)
            {
                case AgentType.Player:
                    playerEnergySystem.IncreaseEnergy(amount);
                    break;
                case AgentType.Enemy:
                    enemyEnergySystem.IncreaseEnergy(amount);
                    break;
            }
        }

        private void DecreaseEnergy(AgentType type, float amount)
        {
            switch (type)
            {
                case AgentType.Player:
                    playerEnergySystem.DecreaseEnergy(amount);
                    break;
                case AgentType.Enemy:
                    enemyEnergySystem.DecreaseEnergy(amount);
                    break;
            }
        }

        [ContextMenu("Add Energy To Player")]
        private void AddEnergy()
        {
            IncreaseEnergy(AgentType.Player, .4f);
        }
        
        
        [ContextMenu("Remove Energy To Player")]
        private void RemoveEnergy()
        {
            DecreaseEnergy(AgentType.Player, .3f);
        }
        
        [ContextMenu("AAAAAAA")]
        private void StartMatchManually()
        {
            playerEnergySystem.StartRecharging();
            enemyEnergySystem.StartRecharging();
        }
        
        public Vector3 GetBallPosition()
        {
            return ball.transform.position;
        }

        public GameObject GetBall()
        {
            return ball;
        }

        public Vector3 GetEnemyFencePosition(MatchSide side)
        {
            switch (side)
            {
                case MatchSide.Attacker when playerStatus == MatchSide.Attacker:
                    return fenceEnemy.transform.position;
                case MatchSide.Attacker when enemyStatus == MatchSide.Attacker:
                    return fencePlayer.transform.position;
                default:
                    return Vector3.zero;
            }
        }

        public BoxCollider SpawnBox(AgentType type)
        {
            switch (type)
            {
                case AgentType.Player:
                    return spawnPlayerArea;
                case AgentType.Enemy:
                    return spawnEnemyArea;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void StartMatch()
        {
            if (playerStatus == MatchSide.Attacker && enemyStatus == MatchSide.Defender)
            {
                SpawnBall(spawnPlayerArea);
            }
            else if (enemyStatus == MatchSide.Attacker && playerStatus == MatchSide.Defender)
            {
                SpawnBall(spawnEnemyArea);
            }
            else
            {
                SpawnBall(spawnPlayerArea);
            }
            
            playerEnergySystem.StartRecharging();
            enemyEnergySystem.StartRecharging();
        }

        private void UpdatePlayerScore(AgentType type)
        {
            switch (type)
            {
                case AgentType.Player:
                    playerScore++;
                    break;
                case AgentType.Enemy:
                    enemyScore++;
                    break;
                case AgentType.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            } 
        }
        
        private void NextMatch()
        {
            playerStatus = playerStatus == MatchSide.Attacker ? MatchSide.Defender : MatchSide.Attacker;
            enemyStatus = enemyStatus == MatchSide.Attacker ? MatchSide.Defender : MatchSide.Attacker;
            currentMatch++;
            isBallOnPlayer = false;
            playerEnergySystem.RestartRecharge();
            enemyEnergySystem.RestartRecharge();
            StartMatch();
        }
        
        private void SpawnBall(BoxCollider spawnArea)
        {
            Vector3 spawnPosition = spawnArea.GetRandomPointInsideCollider();
            spawnPosition.y = 0.2f;
            ball.transform.position = spawnPosition;
            ball.GetComponent<SphereCollider>().enabled = true;
            ball.transform.SetParent(null);
            ball.SetActive(true);
        }

        private void OnCatch(AgentController agent)
        {
            isBallOnPlayer = true;
        }
    }
}