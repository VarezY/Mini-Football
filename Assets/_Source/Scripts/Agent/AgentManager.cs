using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentManager : MonoBehaviour
    {
        private enum AgentType{ Player, Enemy, }
        
        [Header("Object Pool")]
        public List<GameObject> pooledObjects;
        public GameObject objectToPool;
        public int amountToPool;

        [Header("Agent Settings")] 
        public Color playerColor;
        public Color enemyColor;

        private InGameManager _gameManager;
        private AgentType _agentType;
        private Camera _camera;
        private Vector3 _spawnPoint;

        private void Awake()
        {
            _camera = Camera.main;
            
            pooledObjects = new List<GameObject>();
            for(int i = 0; i < amountToPool; i++)
            {
                GameObject tmp = Instantiate(objectToPool);
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

        private void SpawnSelectedAgent(AgentType agentType)
        {
            GameObject selectedAgent = GetPooledObject();
            bool isAgent = selectedAgent.TryGetComponent(out AgentController agentController);
            switch (agentType)
            {
                case AgentType.Player:
                    if (isAgent)
                    {
                        agentController.flagColor = playerColor;
                        agentController.side = _gameManager.matchManager.playerStatus;
                        agentController.state = _gameManager.matchManager.isBallOnPlayer? AgentState.GoToFence : AgentState.Idle;
                    }
                    break;
                case AgentType.Enemy:
                    if (isAgent)
                    {
                        agentController.flagColor = enemyColor;
                        agentController.side = _gameManager.matchManager.enemyStatus;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
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
            // Instantiate(playerPrefab, _spawnPoint, Quaternion.identity);
            // Spawn from object pool
            if (!modifiedAgent) return;

            modifiedAgent.transform.position = _spawnPoint;
            modifiedAgent.transform.rotation = Quaternion.identity;
            modifiedAgent.SetActive(true);
        }
    }
}