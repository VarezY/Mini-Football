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

        private void Start()
        {
            _gameManager = InGameManager.instance;
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
        
        public GameObject GetPooledObject()
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

        public void ResetPooledObjects()
        {
            for (int i = 0; i < objectToPoolParent.childCount; i++)
            {
                objectToPoolParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void SpawnSelectedAgent(AgentType type)
        {
            GameObject selectedAgent = GetPooledObject();
            bool isAgent = selectedAgent.TryGetComponent(out AgentController agentController);
            switch (type)
            {
                case AgentType.Player:
                    if (isAgent)
                    {
                        agentController.flagColor = playerColor;
                        agentController.side = _gameManager.matchManager.playerStatus;
                        _agentRotation = new Vector3(0, 0, 0);
                    }
                    break;
                case AgentType.Enemy:
                    if (isAgent)
                    {
                        agentController.flagColor = enemyColor;
                        agentController.side = _gameManager.matchManager.enemyStatus;
                        _agentRotation = new Vector3(0, 180, 0);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            switch (agentController.side)
            {
                case MatchSide.Attacker:
                    _canSpawn = _gameManager.uiManager.RemoveCharge(type, 2);
                    break;
                case MatchSide.Defender:
                    _canSpawn = _gameManager.uiManager.RemoveCharge(type, 3);
                    break;
            }

            if (!_canSpawn)
            {
                return;
            }

            SpawnAgent(selectedAgent);
        }
        
        private void SpawnAgent(GameObject modifiedAgent)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                _spawnPoint = hit.point;
            }
            _spawnPoint.y = 0f;

            // Spawn from object pool
            if (!modifiedAgent) return;

            modifiedAgent.transform.position = _spawnPoint;
            modifiedAgent.transform.Rotate(_agentRotation);
            modifiedAgent.SetActive(true);
        }

        public void SwitchAgentStatus(AgentController agent)
        {
            if (agent.side == MatchSide.Attacker && agent.state != AgentState.Idle) activeAgents.Add(agent);
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