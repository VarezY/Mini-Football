using System;
using MiniFootball.Agent;
using MiniFootball.Game;
using MiniFootball.UI.Timer;
using TMPro;
using UnityEngine;

namespace MiniFootball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TimerController timerController;

        [Header("Player UI")]
        [SerializeField] private EnergyBarController playerBarController;
        [SerializeField] private TMP_Text playerName;
        
        [Header("Enemy UI")]
        [SerializeField] private EnergyBarController enemyBarController;
        [SerializeField] private TMP_Text enemyName;

        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch += NextMatch;
                
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() => InGameManager.instance.InGameEvents.OnStartGame -= StartMatch);
        }

        private void StartMatch()
        {
            timerController.StartTimer();
            playerBarController.StartRecharge();
            enemyBarController.StartRecharge();
        }

        private void NextMatch()
        {
            timerController.RestartTimer();
            playerBarController.ResetEnergyBars();
            enemyBarController.ResetEnergyBars();
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
    }
}