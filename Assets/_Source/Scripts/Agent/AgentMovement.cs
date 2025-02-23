using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1.5f;
        public float moveSpeedWithBall = 0.75f;
        public float moveSpeedNormal = 0.75f;
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

        // Move towards a specific world position
        public void MoveToPosition(Vector3 targetPosition)
        {
            // Get direction to target
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0; // Keep movement on the horizontal plane

            if (!(direction.magnitude > stoppingDistance)) return;
            
            // Normalize for consistent speed
            direction = direction.normalized;
            
            // Move towards target
            _controller.Move(direction * (moveSpeed * Time.deltaTime));
            
            // Optional: Rotate towards movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}