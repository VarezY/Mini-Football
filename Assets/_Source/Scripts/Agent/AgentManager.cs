using System;
using System.Collections.Generic;
using MiniFootball.Game;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private int attackerSpawnEnergy;
        [SerializeField] private int defenderSpawnEnergy;
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
                SpawnAgentOnField();
            }
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

        private void SpawnAgentOnField()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                _spawnPoint = hit.point;
            }
            _spawnPoint.y = 0f;
            
            if (!hit.transform.TryGetComponent(out BoxCollider spawnArea)) return;
            
            // Constructor for an Agent
            AgentType agentType = AgentType.Null;
            MatchSide side;
            Color flagColor;
            Vector3 faceDirection;
            
            // Set the variables
            if (spawnArea == _boxPlayer)
            {
                agentType = AgentType.Player;
                side = _gameManager.matchManager.playerStatus;
                flagColor = playerColor;
                faceDirection = new Vector3(0, 0, 0);
            }
            else if (spawnArea == _boxEnemy)
            {
                agentType = AgentType.Enemy;
                side = _gameManager.matchManager.enemyStatus;
                flagColor = enemyColor;
                faceDirection = new Vector3(0, 180, 0);
            }
            else return;

            // Construct it
            GameObject selectedAgent = ModifyAgent(agentType, side, flagColor);
            if (!selectedAgent) return;
            
            // Check if the energy is available
            bool canSpawn = false;
            switch (side)
            {
                case MatchSide.Attacker when agentType == AgentType.Player:
                    canSpawn = _gameManager.matchManager.CanSpawn(AgentType.Player, attackerSpawnEnergy);
                    break;
                case MatchSide.Defender when agentType == AgentType.Player:
                    canSpawn = _gameManager.matchManager.CanSpawn(AgentType.Player, defenderSpawnEnergy);
                    break;
                case MatchSide.Attacker when agentType == AgentType.Enemy:
                    canSpawn = _gameManager.matchManager.CanSpawn(AgentType.Enemy, attackerSpawnEnergy);
                    break;
                case MatchSide.Defender when agentType == AgentType.Enemy:
                    canSpawn = _gameManager.matchManager.CanSpawn(AgentType.Enemy, defenderSpawnEnergy);
                    break;
            }
            
            if (!canSpawn) return;
            
            
            // Spawn it
            selectedAgent.transform.position = _spawnPoint;
            selectedAgent.transform.Rotate(faceDirection);
            selectedAgent.SetActive(true);
        }

        private GameObject ModifyAgent(AgentType type, MatchSide side, Color flagColor)
        {
            GameObject selectedAgent = GetPooledObject();
            bool isAgent = selectedAgent.TryGetComponent(out AgentController agentController);
            if (!isAgent) return null;
            
            agentController.SetBallPosition(_gameManager.matchManager.GetBallPosition());
            agentController.agentType = type;
            agentController.flagColor = flagColor;
            agentController.side = side;
            return selectedAgent;

        }
    }
}