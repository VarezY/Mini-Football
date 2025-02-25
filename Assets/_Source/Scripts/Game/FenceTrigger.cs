using System;
using MiniFootball.Agent;
using UnityEngine;

namespace MiniFootball.Game
{
    public class FenceTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            if (!other.TryGetComponent(out AgentController agentController)) return;
            agentController.ResetAgent();
            agentController.gameObject.SetActive(false);
            
            if (!agentController.hasBall) return;
            InGameManager.instance.NextMatch(agentController.agentType);
            Debug.Log($"Scored by {other.name}");
        }
    }    
}

