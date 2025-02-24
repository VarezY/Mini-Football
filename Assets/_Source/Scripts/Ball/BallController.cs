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
            if (!other.TryGetComponent(out Agent.AgentController agent)) return;
            if (agent.side != MatchSide.Attacker) return;
            if (agent.state == AgentState.Idle) return;
            
            InGameManager.instance.InGameEvents.BallCatch(agent);
            this.transform.parent = other.transform;
            this.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
        }
    }
}

