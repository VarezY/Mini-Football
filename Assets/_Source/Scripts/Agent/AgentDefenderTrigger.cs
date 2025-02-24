using System;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentDefenderTrigger : MonoBehaviour
    {
        private AgentController _agentController;
        
        private void Awake()
        {
            _agentController = GetComponentInParent<AgentController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;
            
            if (other.TryGetComponent(out AgentController agentController))
            {
                if (!agentController.hasBall) return;
                
                Debug.Log($"Player {other.name} trigger entered with Ball");
                _agentController.ChaseAgentWithBall(agentController);
            }
            
        }
    }
}
