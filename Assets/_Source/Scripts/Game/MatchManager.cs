using System;
using UnityEngine;

namespace MiniFootball.Game
{
    public class MatchManager : MonoBehaviour
    {
        public enum Side { Attacker, Defender }
        public enum GameState { Starting, Playing, Scoring }
        
        [Header("Debug Info (Read Only)")]
        [SerializeField] private int currentMatch = 1;
        [SerializeField] private GameState currentState = GameState.Starting;
        [SerializeField] private Side playerStatus = Side.Attacker;
        [SerializeField] private Side enemyStatus = Side.Defender;

        [Header("Match Configuration")]
        [SerializeField] private GameObject ball;
        [SerializeField] private int maxMatches = 6;
        [SerializeField] private int timer = 140;
        public int timeToFillEnergy = 2;
        
        [Header("Player Configuration")]
        [SerializeField] private BoxCollider spawnPlayerArea;
        [SerializeField] private GameObject playerPrefab;

        [Header("Enemy Configuration")]
        [SerializeField] private BoxCollider spawnEnemyArea;
        [SerializeField] private GameObject enemyPrefab;

        private Camera _camera;
        private Vector3 _allocPos;
        private int _tmp;

        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() => 
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() => InGameManager.instance.InGameEvents.OnStartGame -= StartMatch);
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnAgent();
            }
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

        [ContextMenu("Start Match")]
        private void StartMatch()
        {
            if (playerStatus == Side.Attacker && enemyStatus == Side.Defender)
            {
                SpawnBall(spawnPlayerArea);
            }
            else if (enemyStatus == Side.Attacker && playerStatus == Side.Defender)
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
            
        }

        private void SpawnAgent()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                _allocPos = hit.point;
            }
            _allocPos.y = 0f;
            Instantiate(playerPrefab, _allocPos, Quaternion.identity);
        }
    }
}