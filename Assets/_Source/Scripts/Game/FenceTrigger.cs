using System;
using MiniFootball.Agent;
using UnityEngine;

namespace MiniFootball.Game
{
    public class FenceTrigger : MonoBehaviour
    {
        [SerializeField] private AgentType fenceType;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            if (!other.TryGetComponent(out AgentController agentController)) return;
            agentController.ResetAgent();
            
            if (!agentController.hasBall) return;
            if (agentController.agentType == fenceType) return;
            
            InGameManager.instance.NextMatch(agentController.agentType);
            Debug.Log($"Scored by {other.name}");
        }
    }    
}

