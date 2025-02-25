using System;
using System.Collections.Generic;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentManager : MonoBehaviour
    {
        [Header("Object Pool")]
        public List<GameObject> pooledObjects;
        public GameObject objectToPool;
        public Transform objectToPoolParent;
        public int amountToPool;

        [Header("Agent Settings")] 
        public Color playerColor;
        public Color enemyColor;

        [Header("Agent AttackerStatus")]
        public List<AgentController> activeAgents;
        public List<AgentController> inactiveAgents;

        private InGameManager _gameManager;
        private AgentType _agentType;
        private Camera _camera;
        private BoxCollider _boxPlayer, _boxEnemy;
        private Vector3 _spawnPoint;
        private Vector3 _agentRotation;
        private bool _canSpawn;

        private void Awake()
        {
            _camera = Camera.main;
            activeAgents = new List<AgentController>(35);
            inactiveAgents = new List<AgentController>(35);
            
            pooledObjects = new List<GameObject>();
            for(int i = 0; i < amountToPool; i++)
            {
                GameObject tmp = Instantiate(objectToPool, objectToPoolParent, true);
                tmp.name = $"Agent-{i}";
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnNextMatch += ResetPooledObjects;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnNextMatch -= ResetPooledObjects;

            });
        }

        private void Start()
        {
            _gameManager = InGameManager.instance;
            _boxPlayer = _gameManager.matchManager.SpawnBox(AgentType.Player);
            _boxEnemy = _gameManager.matchManager.SpawnBox(AgentType.Enemy);
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnSelectedAgent(AgentType.Player);
            }

            if (Input.GetMouseButtonDown(1))
            {
                SpawnSelectedAgent(AgentType.Enemy);
            }
        }

        private GameObject GetPooledObject()
        {
            for(int i = 0; i < amountToPool; i++)
            {
                if(!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            return null;
        }

        private void ResetPooledObjects()
        {
            activeAgents.Clear();
            inactiveAgents.Clear();
            for (int i = 0; i < objectToPoolParent.childCount; i++)
            {
                objectToPoolParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void SpawnSelectedAgent(AgentType type)
        {
            GameObject selectedAgent = GetPooledObject();
            bool isAgent = selectedAgent.TryGetComponent(out AgentController agentController);
            agentController.SetBallPosition(_gameManager.matchManager.GetBallPosition());
            switch (type)
            {
                case AgentType.Player:
                    if (isAgent)
                    {
                        agentController.agentType = AgentType.Player;
                        agentController.flagColor = playerColor;
                        agentController.side = _gameManager.matchManager.playerStatus;
                        _agentRotation = new Vector3(0, 0, 0);
                    }
                    break;
                case AgentType.Enemy:
                    if (isAgent)
                    {
                        agentController.agentType = AgentType.Enemy;
                        agentController.flagColor = enemyColor;
                        agentController.side = _gameManager.matchManager.enemyStatus;
                        _agentRotation = new Vector3(0, 180, 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            SpawnAgent(selectedAgent, agentController.side, type);
        }
        
        private void SpawnAgent(GameObject modifiedAgent, MatchSide side, AgentType type)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                _spawnPoint = hit.point;
            }
            _spawnPoint.y = 0f;
           
            if (!modifiedAgent) return;

            if (!hit.transform.TryGetComponent<BoxCollider>(out BoxCollider spawnArea)) return;

            switch (type)
            {
                case AgentType.Player when spawnArea == _boxPlayer:
                    break;
                case AgentType.Enemy when spawnArea == _boxEnemy:
                    break;
                default:
                    return;
            }

            switch (side)
            {
                case MatchSide.Attacker:
                    _canSpawn = _gameManager.uiManager.RemoveCharge(type, 2);
                    break;
                case MatchSide.Defender:
                    _canSpawn = _gameManager.uiManager.RemoveCharge(type, 3);
                    break;
            }
            
            if (!_canSpawn) return;

            // Spawn from object pool
            modifiedAgent.transform.position = _spawnPoint;
            modifiedAgent.transform.Rotate(_agentRotation);
            modifiedAgent.SetActive(true);
        }

        public void SwitchAgentStatus(AgentController agent)
        {
            activeAgents.Add(agent);
        }

        public AgentController ClosestDistance(AgentController agentPivot)
        {
            float closestDistance = Mathf.Infinity;
            AgentController closest = null;
            foreach (AgentController agent in activeAgents)
            {
                float distance = Vector3.Distance(agentPivot.transform.position, agent.transform.position);
                if (distance > closestDistance) continue;
                
                closestDistance = distance;
                closest = agent;
            }

            return closest;
        }
    }
}