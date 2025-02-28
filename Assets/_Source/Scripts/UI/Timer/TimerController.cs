using System;
using DG.Tweening;
using MiniFootball.Agent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiniFootball.UI.Timer
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private Image timerOutline;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private UnityEvent onTimerEnd;
        
        private InGameManager _gameManager;
        private Tween _outlineTween;
        private Tween _timerTween;
        private int _timer;

        private void Start()
        {
            _gameManager = InGameManager.instance;
        }

        [ContextMenu("Start")]
        public void StartTimer()
        {
            _timer = _gameManager.matchManager.timer;
            _outlineTween = timerOutline.DOFillAmount(0, _timer).SetEase(Ease.Linear);
            _timerTween = DOVirtual.Int(_timer, 0, _timer, value => timerText.text = $"{value}")
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    onTimerEnd?.Invoke();
                });
        }

        public void DrawGame()
        {
            _gameManager.NextMatch(AgentType.Null);
        }

        public void RestartTimer()
        {
            timerOutline.DOFillAmount(1, 0);
            _outlineTween?.Kill();
            _timerTween?.Kill();
            StartTimer();
        }

        public void StopTimer()
        {
            _outlineTween?.Kill();
            _timerTween?.Kill();
        }
    }
}