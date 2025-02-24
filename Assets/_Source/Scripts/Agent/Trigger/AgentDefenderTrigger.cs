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

            if (!other.TryGetComponent(out AgentController agentController)) return;
            
            if (!agentController.hasBall) return;
                
            _agentController.ChaseAgentWithBall(agentController);

        }

        // TODO: FIX WHEN AGENT IS IN THE AREA AND RECIEVE BALL
        /*private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            if (!other.TryGetComponent(out AgentController agentController)) return;
            
            if (!agentController.hasBall) return;
                
            _agentController.ChaseAgentWithBall(agentController);
        }*/
    }
}
