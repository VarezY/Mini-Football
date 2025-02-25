using System;
using MiniFootball.Agent;
using MiniFootball.Game;
using MiniFootball.UI;
using UnityEngine;

namespace MiniFootball
{
    public class InGameManager : MonoBehaviour
    {
        public static InGameManager instance { get; private set; }

        public InGameEvents InGameEvents;
        public UIManager uiManager { get; private set; }
        public MatchManager matchManager { get; private set; }
        public AgentManager agentManager { get; private set; }

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            InGameEvents = new InGameEvents();

            uiManager = GetComponentInChildren<UIManager>();
            matchManager = GetComponentInChildren<MatchManager>();
            agentManager = GetComponentInChildren<AgentManager>();
        }

        private void Start()
        {
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            InGameEvents.StartGame();
        }

        public void NextMatch(AgentType scoredAgent)
        {
            InGameEvents.ScoredGoal(scoredAgent);
            if (matchManager.currentMatch == matchManager.maxMatches)
            { 
                InGameEvents.EndGame();
            }
            else
            {
                InGameEvents.NextMatch();
            }
        }
    }
}
