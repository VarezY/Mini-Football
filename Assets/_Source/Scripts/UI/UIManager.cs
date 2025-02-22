using System;
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
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch));
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
    }
}