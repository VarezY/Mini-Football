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
            if (!hasController) return;

            switch (_agentController.side)
            {
                case MatchSide.Attacker when collideAgent.side != MatchSide.Attacker:
                    _agentController.InactiveAgent();
                    break;
                case MatchSide.Defender when collideAgent.side != MatchSide.Defender:
                    _agentController.ReturnToPatrol();
                    break;
            }

            // DO IT AGAIN TO BE SAFE
            switch (collideAgent.side)
            {
                case MatchSide.Attacker when _agentController.side != MatchSide.Attacker:
                    collideAgent.InactiveAgent();
                    break;
                case MatchSide.Defender when _agentController.side != MatchSide.Defender:
                    collideAgent.ReturnToPatrol();
                    break;
            }
        }
    }
}