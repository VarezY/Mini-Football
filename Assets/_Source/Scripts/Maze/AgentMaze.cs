using System;
using MiniFootball.Agent;
using UnityEngine;

namespace MiniFootball.Maze
{
    public class AgentMaze : MonoBehaviour
    {
        public enum MazeDirection { Left, Right }
        public SphereCollider sphereCollider;
        public AgentMovement agentMovement;
        public MazeDirection mazeDirection;
        public bool keepWalking;

        public void MoveAgentLeft()
        {
        }

        public void MoveAgentRight()
        {
            
        }

        private void Update()
        {
            if (!keepWalking) return;

            switch (mazeDirection)
            {
                case MazeDirection.Left:
                    agentMovement.MoveTowardsDirection(new Vector3(-1, 0, 0));
                    break;
                case MazeDirection.Right:
                    agentMovement.MoveTowardsDirection(new Vector3(1, 0, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }    
}

