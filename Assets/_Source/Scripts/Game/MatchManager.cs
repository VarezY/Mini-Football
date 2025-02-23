using System;
using UnityEngine;

namespace MiniFootball.Game
{
    public class MatchManager : MonoBehaviour
    {
        public enum GameState { Starting, Playing, Scoring }
        
        [Header("Debug Info (Read Only)")]
        [SerializeField] private int currentMatch = 1;
        [SerializeField] private GameState currentState = GameState.Starting;
        public MatchSide playerStatus = MatchSide.Attacker;
        public MatchSide enemyStatus = MatchSide.Defender;
        public bool isBallOnPlayer;

        [Header("Match Configuration")]
        [SerializeField] private GameObject ball;
        [SerializeField] private int maxMatches = 6;
        public int timer = 140;
        public int timeToFillEnergy = 2;
        
        [Header("Player Configuration")]
        [SerializeField] private BoxCollider spawnPlayerArea;
        [SerializeField] private BoxCollider fencePlayer;
        [SerializeField] private GameObject playerPrefab;

        [Header("Enemy Configuration")]
        [SerializeField] private BoxCollider spawnEnemyArea;
        [SerializeField] private BoxCollider fenceEnemy;
        [SerializeField] private GameObject enemyPrefab;

        private Camera _camera;
        private Vector3 _allocPos;
        private int _tmp;

        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch += NextMatch;
                InGameManager.instance.InGameEvents.OnBallCatch += controller => isBallOnPlayer = true;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame -= StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch -= NextMatch;
            });
        }

        private void Start()
        {
            _camera = Camera.main;
        }
        
        public void SpawnBall(BoxCollider spawnArea)
        {
            Vector3 spawnPosition = spawnArea.GetRandomPointInsideCollider();
            spawnPosition.y = 0.2f;
            ball.transform.position = spawnPosition;
            ball.SetActive(true);
        }

        public Vector3 GetBallPosition()
        {
            return ball.transform.position;
        }

        public Vector3 GetFencePosition(MatchSide side)
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

        [ContextMenu("Start Match")]
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
        }

        private void NextMatch()
        {
            playerStatus = playerStatus == MatchSide.Attacker ? MatchSide.Defender : MatchSide.Attacker;
            enemyStatus = enemyStatus == MatchSide.Attacker ? MatchSide.Defender : MatchSide.Attacker;
            currentMatch++;
            StartMatch();
        }
    }
}