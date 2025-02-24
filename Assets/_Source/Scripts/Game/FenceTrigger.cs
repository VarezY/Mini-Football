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
            Destroy(agentController.gameObject);
            
            if (!agentController.hasBall) return;
            Debug.Log($"Scored by {other.name}");
        }
    }    
}

