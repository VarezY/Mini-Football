﻿using System;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1.5f;
        public float moveSpeedWithBall = 0.75f;
        public float moveSpeedNormal = 0.75f;
        public float moveSpeedDefender = 1f;
        public float returnSpeedDefender = 2f;
        public float speedScaler = 0.25f;

        public Vector3 direction;
        public Vector3 velocity;
        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void MoveToPosition(Vector3 targetPosition, Action onDestinationReached = null)
        {
            bool ar = InGameManager.instance.arManager.inAR;
            direction = (targetPosition - transform.localPosition);
            direction.y = 0; // Lock y axis

            if (!(direction.magnitude > 0.0025f))
            {
                onDestinationReached?.Invoke();
                return;
            }
            
            direction = direction.normalized;
            if (ar)
            {
                velocity = direction * (moveSpeed * 0.1f * Time.deltaTime);
            }
            else
            {
                velocity = direction * (moveSpeed * Time.deltaTime);
            }
            _controller.Move(velocity);
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void MoveTowardsDirection(Vector3 targetPosition)
        {
            _controller.Move(targetPosition * (moveSpeed * Time.deltaTime));
        }
    }
}

