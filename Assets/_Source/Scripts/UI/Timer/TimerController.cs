using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFootball.UI.Timer
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private Image timerOutline;
        [SerializeField] private TMP_Text timerText;
        
        private InGameManager _gameManager;
        private int _timer;

        private void Start()
        {
            _gameManager = InGameManager.instance;
        }

        [ContextMenu("Start")]
        public void StartTimer()
        {
            _timer = _gameManager.matchManager.timer;
            timerOutline.DOFillAmount(0, _timer).SetEase(Ease.Linear);
            DOVirtual.Int(_timer, 0, _timer, value => timerText.text = $"{value}").SetEase(Ease.Linear);
        }

        public void RestartTimer()
        {
            timerOutline.fillAmount = 1;
            StartTimer();
        }
        
    }
}