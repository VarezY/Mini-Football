using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniFootball.Game.AR
{
    public class SpawnBall : MonoBehaviour
    {
        [Header("Camera AR")]
        public Camera mainCamera;
        public Camera ARCamera;
        
        [Header("Game")]
        public GameObject ballPrefab;
        public GameObject checker;
        public float scaleAR;
        public List<GameObject> pooledObjects;
        private Camera _camera;
        private Vector3 _spawnPoint;

        private void Awake()
        {
            pooledObjects = new List<GameObject>();
            GameObject tmp;
            for(int i = 0; i < 55; i++)
            {
                tmp = Instantiate(ballPrefab, this.transform);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        private void Start()
        {
            _camera = mainCamera;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnTheBall();
            }
        }

        private void SpawnTheBall()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                _spawnPoint = hit.point;
            }

            checker.transform.position = _spawnPoint;
            
            GameObject ball = GetPooledObject();
            ball.transform.position = _spawnPoint;
            ball.SetActive(true);
        }
        
        private GameObject GetPooledObject()
        {
            for(int i = 0; i < 55; i++)
            {
                if(!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            return null;
        }

        public void ChangeCameraToAR()
        {
            _camera = ARCamera;
        }

        public void ChangeToBasic()
        {
            _camera = mainCamera;
        }
    }
}