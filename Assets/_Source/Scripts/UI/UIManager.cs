using System;
using DG.Tweening;
using MiniFootball.Agent;
using MiniFootball.UI.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniFootball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TimerController timerController;

        [Header("Game UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Text winnerText;
        [SerializeField] private GameObject playAgainButton;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private RectTransform timerPanel;
        [SerializeField] private RectTransform scorePanel;

        [Header("Player UI")]
        [SerializeField] private RectTransform playerPanel;
        [SerializeField] private TMP_Text playerName;
        
        [Header("Enemy UI")]
        [SerializeField] private RectTransform enemyPanel;
        [SerializeField] private TMP_Text enemyName;

        private Sequence _uiSequence;
        private int _playerScore;
        private int _enemyScore;
        
        private void OnEnable()
        {
            StartCoroutine(this.WaitAndSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame += StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch += NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal += UpdatePlayerScore;
                InGameManager.instance.InGameEvents.OnEndGame += ShowWinner;
            }));
        }

        private void OnDisable()
        {
            this.WaitAndUnSubscribe(() =>
            {
                InGameManager.instance.InGameEvents.OnStartGame -= StartMatch;
                InGameManager.instance.InGameEvents.OnNextMatch -= NextMatch;
                InGameManager.instance.InGameEvents.OnScoredGoal -= UpdatePlayerScore;
                InGameManager.instance.InGameEvents.OnEndGame -= ShowWinner;
            });
        }

        private void Start()
        {
            _uiSequence = DOTween.Sequence();
            
            //local position in Debug Inspector
            playerPanel.DOLocalMoveX(465f, 2f).SetDelay(4.5f); 
            enemyPanel.DOLocalMoveX(-465f, 2f).SetDelay(4.5f); 
            timerPanel.DOLocalMoveY(860, 2f).SetDelay(4.5f);
            scorePanel.DOLocalMoveY(799.8f, 2f).SetDelay(4.5f);
        }

        private void StartMatch()
        {
            timerController.StartTimer();
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
        }

        private void ShowWinner()
        {
            timerController.StopTimer();
            if (_playerScore > _enemyScore)
            {
                Debug.Log("Player WIN the game");
                AnimateWin($"GAME OVER\n<color=red>PLAYER</color> WIN!");
            }
            else if (_playerScore < _enemyScore)
            {
                AnimateWin($"GAME OVER\n<color=blue>ENEMY</color> WIN!");
                Debug.Log("Enemy WIN the game");
            }
            else
            {
                AnimateWin("GAME OVER\nDRAW", () =>
                {
                    InGameManager.instance.PenaltyGame();
                });
                Debug.Log("GAME END IN DRAW - GO TO PENALTY");
            }
        }

        public void AnimateWin(string winText)
        {
            winnerText.text = winText;
            winnerText.rectTransform.DOScale(2f, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    playAgainButton.SetActive(true);
                    exitButton.SetActive(true);
                });
        }
        
        private void AnimateWin(string winText, Action onComplete)
        {
            winnerText.text = winText;
            winnerText.rectTransform.DOScale(2f, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(2f, () => onComplete?.Invoke());
                });
        }
    }
}