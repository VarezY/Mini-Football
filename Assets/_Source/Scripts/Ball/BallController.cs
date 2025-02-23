using System;
using MiniFootball.Agent;
using UnityEngine;

namespace MiniFootball.Ball
{
    public class BallController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;
            
            bool isAgent = other.TryGetComponent(out AgentController agent);
            this.transform.parent = other.transform;
            this.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
            if (isAgent) InGameManager.instance.InGameEvents.BallCatch(agent);
        }
    }
}

