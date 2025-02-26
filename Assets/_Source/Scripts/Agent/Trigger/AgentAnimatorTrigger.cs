using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentAnimatorTrigger : MonoBehaviour
    {
        private void OnFinishCelebrate()
        {
            this.transform.parent.gameObject.SetActive(false);
        }

        private void OnPass()
        {
            
        }

        private void OnTackle()
        {
            AgentController a = GetComponentInParent<AgentController>();
            a.state = AgentState.ReturnToPatrol;

        }
    }
}