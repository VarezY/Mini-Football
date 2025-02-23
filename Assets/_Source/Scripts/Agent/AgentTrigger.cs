using System;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentTrigger : MonoBehaviour
    {
        private AgentController _agentController;
        
        private void Awake()
        {
            _agentController = GetComponentInParent<AgentController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (!TryGetComponent(out AgentController agentController)) return;
            if (agentController.hasBall)
            {
                _agentController.ChaseAgentWithBall(agentController);
            }
        }
    }
}
