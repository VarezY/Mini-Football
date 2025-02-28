using System;
using MiniFootball.Agent;
using MiniFootball.Game;
using MiniFootball.Game.AR;
using MiniFootball.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniFootball
{
    public class InGameManager : MonoBehaviour
    {
        public static InGameManager instance { get; private set; }

        public InGameEvents InGameEvents;
        public UIManager uiManager { get; private set; }
        public MatchManager matchManager { get; private set; }
        public AgentManager agentManager { get; private set; }
        public ARManager arManager { get; private set; }

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            InGameEvents = new InGameEvents();

            arManager = FindObjectOfType<ARManager>();
            uiManager = GetComponentInChildren<UIManager>();
            matchManager = GetComponentInChildren<MatchManager>();
            agentManager = GetComponentInChildren<AgentManager>();
        }

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

        public void PenaltyGame()
        {
            SceneManager.LoadScene("_Source/Scene/Game Maze");

        }

        public void RestartGame()
        {
            SceneManager.LoadScene("_Source/Scene/Game");
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("_Source/Scene/Main Menu");
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }
}
