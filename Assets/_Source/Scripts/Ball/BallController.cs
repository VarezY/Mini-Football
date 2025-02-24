using System;
using MiniFootball.Agent;
using MiniFootball.Game;
using UnityEngine;

namespace MiniFootball.Ball
{
    public class BallController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;
            
            bool isAgent = other.TryGetComponent(out AgentController agent);
            
            if (!isAgent) return;
            
            if (agent.side == MatchSide.Attacker)
            {
                InGameManager.instance.InGameEvents.BallCatch(agent);
                this.transform.parent = other.transform;
                this.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
            }
        }
    }
}

