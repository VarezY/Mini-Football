using System;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentCollisionTrigger : MonoBehaviour
    {
        private AgentController _agentController;
        private bool _hasHit;
        
        private void Awake()
        {
            _agentController = GetComponentInParent<AgentController>();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!hit.gameObject.CompareTag("Agent")) return;
            
            if (_hasHit) return; //to ensure only once
            _hasHit = true;

            bool hasController = hit.gameObject.TryGetComponent(out AgentController collideAgent);
           
            switch (_agentController.side)
            {
                case MatchSide.Attacker:
                    Debug.Log($"{_agentController.name} get caught: PASS");
                    _agentController.InactiveAgent();
                    break;
                case MatchSide.Defender:
                    Debug.Log($"{_agentController.name} Get back to initial position");
                    _agentController.ReturnToPatrol();
                    break;
            }

            if (!hasController) return;
            
            switch (collideAgent.side)
            {
                case MatchSide.Attacker:
                    Debug.Log($"{collideAgent.name} get caught: PASS");
                    collideAgent.InactiveAgent();
                    break;
                case MatchSide.Defender:
                    Debug.Log($"{collideAgent.name} Get back to initial position");
                    collideAgent.ReturnToPatrol();
                    break;
            }
        }
    }
}