using System;
using MiniFootball.Agent;
using MiniFootball.UI.Timer;
using TMPro;
using UnityEngine;

namespace MiniFootball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TimerController timerController;

        [Header("Game UI")]
        [SerializeField] private TMP_Text scoreText;
        
        [Header("Player UI")]
        [SerializeField] private EnergyBarController playerBarController;
        [SerializeField] private TMP_Text playerName;
        
        [Header("Enemy UI")]
        [SerializeField] private EnergyBarController enemyBarController;
        [SerializeField] private TMP_Text enemyName;

        private int _playerScore;
        private int _enemyScore;
        
        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch += NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal += UpdatePlayerScore;
                
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame -= StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch -= NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal -= UpdatePlayerScore;
            });
        }
        
        public bool RemoveCharge(AgentType type, int charge)
        {
            switch (type)
            {
                case AgentType.Player when charge <= playerBarController.ActiveEnergyBars.Count:
                    playerBarController.RemoveCharge(charge);
                    return true;
                case AgentType.Enemy when charge <= enemyBarController.ActiveEnergyBars.Count:
                    enemyBarController.RemoveCharge(charge);
                    return true;
                default:
                    return false;
            }
        }

        private void StartMatch()
        {
            timerController.StartTimer();
            playerBarController.StartRecharge();
            enemyBarController.StartRecharge();
        }

        private void UpdatePlayerScore(AgentType type)
        {
            switch (type)
            {
                case AgentType.Player:
                    _playerScore++;
                    break;
                case AgentType.Enemy:
                    _enemyScore++;
                    break;
                case AgentType.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            scoreText.text = $"<color=red>{_playerScore}<color=black> - <color=blue>{_enemyScore}";
        }
        
        private void NextMatch()
        {
            timerController.RestartTimer();
            playerBarController.ResetEnergyBars();
            enemyBarController.ResetEnergyBars();
        }
    }
}