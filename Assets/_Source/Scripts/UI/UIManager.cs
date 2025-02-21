using System;
using MiniFootball.UI.Timer;
using UnityEngine;

namespace MiniFootball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TimerController timerController;

        [Header("Player UI")]
        [SerializeField] private EnergyBarController playerBarController;
        
        [Header("Enemy UI")]
        [SerializeField] private EnergyBarController enemyBarController;

        private void Start()
        {
            timerController.StartTimer();
            playerBarController.StartRecharge();
            enemyBarController.StartRecharge();
        }
    }
}