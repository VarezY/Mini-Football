using System;
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
        [SerializeField] private float stoppingDistance = 0.1f;

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
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0; // Lock y axis

            if (!(direction.magnitude > stoppingDistance))
            {
                onDestinationReached?.Invoke();
                return;
            }
            
            direction = direction.normalized;
            
            _controller.Move(direction * (moveSpeed * Time.deltaTime));
            
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