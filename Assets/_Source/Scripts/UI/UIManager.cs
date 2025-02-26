using System;
using DG.Tweening;
using MiniFootball.Agent;
using MiniFootball.UI.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFootball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TimerController timerController;

        [Header("Game UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Text winnerText;
        [SerializeField] private GameObject playAgainButotn, exitButton;
        
        [Header("Player UI")]
        [SerializeField] private TMP_Text playerName;
        
        [Header("Enemy UI")]
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
        
        private void StartMatch()
        {
            timerController.StartTimer();
            // playerBarController.StartRecharge();
            // enemyBarController.StartRecharge();
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
                Debug.Log("GAME END IN DRAW - GO TO PENALTY");
            }
        }

        private void AnimateWin(string winText)
        {
            winnerText.text = winText;
            winnerText.rectTransform.DOScale(2f, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    playAgainButotn.SetActive(true);
                    exitButton.SetActive(true);
                });
        }
    }
}