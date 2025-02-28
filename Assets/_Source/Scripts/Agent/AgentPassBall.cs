using UnityEngine;

namespace MiniFootball.Agent
{
    public class AgentPassBall : MonoBehaviour
    {
        [SerializeField] private float passSpeed = 1.5f; // Speed at which the ball is passed

        private GameObject _ball;
        private Vector3 _passTargetPosition;
        private AgentController _targetAgent;
        private bool _isPassing = false;

        private void Start()
        {
            InGameManager.instance.InGameEvents.OnSwitchAR += ScaleBallMovement;
            _ball = InGameManager.instance.matchManager.GetBall();
        }
        
        private void Update()
        {
            if (!_isPassing) return;
            
            // Move the ball toward the target position
            _ball.transform.position = Vector3.MoveTowards(_ball.transform.position, _passTargetPosition, passSpeed * Time.deltaTime);

            // Check if the ball has reached the target position
            if (Vector3.Distance(_ball.transform.position, _passTargetPosition) < 0.1f)
            {
                CompletePass();
            }
        }
        
        // Start the pass to the target agent
        public void StartPass(AgentController targetAgent)
        {
            if (!_ball || !targetAgent) return;
            _targetAgent = targetAgent;
            
            // Prediction untuk agent yg tetep maju ketika passing
            /*float distance = Vector3.Distance(ball.transform.position, targetAgent.transform.position);
            float timeToReach = distance / passSpeed;
            Vector3 targetOffset = timeToReach * targetAgent.controller.velocity * 1.5f;*/

            _passTargetPosition = targetAgent.transform.position;
            
            _ball.gameObject.GetComponent<SphereCollider>().enabled = false;
            _isPassing = true;
        }

        // Complete the pass
        private void CompletePass()
        {
            _ball.transform.SetParent(_targetAgent.transform);
            InGameManager.instance.InGameEvents.BallCatch(_targetAgent);
            _ball.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
            _isPassing = false;
        }
        
        private void ScaleBallMovement(bool inAR)
        {
            if (inAR)
            {
                passSpeed = passSpeed * 0.1f;
            }
            else
            {
                passSpeed = 1.5f;
            }
        }

    }
}