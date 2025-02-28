using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace MiniFootball.Game.AR
{
    public class ARSpawner : MonoBehaviour
    {
        [Header("AR Configuration")]
        public ARRaycastManager raycastManager;
        static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        [Header("Game Configuration")] 
        public GameObject gameBoard;

        private InGameManager _gameManager;
        private bool _hasSpawned;

        private void Start()
        {
            _gameManager = InGameManager.instance;
            _gameManager.InGameEvents.OnSwitchAR += InGameEventsOnOnSwitchAR;
        }

        private void OnDestroy()
        {
            _gameManager.InGameEvents.OnSwitchAR -= InGameEventsOnOnSwitchAR;
        }

        private void Update()
        {
            if (_hasSpawned) 
                return;

            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;
            
            if (raycastManager.Raycast(touchPosition, _hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = _hits[0].pose;

                if (!gameBoard) return;
                
                gameBoard.transform.position = hitPose.position;
                gameBoard.transform.localScale = Vector3.one * _gameManager.arManager.aRScale;
                gameBoard.SetActive(true);
                raycastManager.enabled = false;
                _hasSpawned = true;
            }
        }

        public void ResetAR()
        {
            _hasSpawned = false;
        }

        private bool TryGetTouchPosition(out Vector2 touchPosition)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                touchPosition = new Vector2(mousePosition.x, mousePosition.y);
                return true;
            }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif
            touchPosition = default;
            return false;
        }
        
        private void InGameEventsOnOnSwitchAR(bool inAR)
        {
            if (inAR) return;
            
            ResetAR();
            gameBoard.transform.position = Vector3.zero;
            gameBoard.transform.localScale = Vector3.one;
            gameBoard.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}