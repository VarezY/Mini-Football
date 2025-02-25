using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace MiniFootball.Game
{
    public class OpeningSequence : MonoBehaviour
    {
        public CinemachineVirtualCamera openingCamera;
        public CinemachineVirtualCamera gameCamera;
        public InGameManager gameManager;

        [Header("Settings")] public GameObject ball;
        public float openingSpeed = 1.5f;

        private void Start()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.5f);
            sequence.Append(DOVirtual.Float(1, 12, 5f, value =>
                {
                    openingCamera.m_Lens.OrthographicSize = value;
                })
                .SetEase(Ease.InQuart));
            sequence.AppendInterval(2f);
            sequence.AppendCallback(() =>
            {
                gameCamera.m_Priority = 100;
                openingCamera.gameObject.SetActive(false);
            });

            sequence.AppendInterval(2.5f);
            sequence.AppendCallback(() =>
            {
                gameManager.StartGame();
            });

        }

        [ContextMenu("Opening Sequence")]
        private void Opening()
        {
            
            DOVirtual.Float(1, 12, 5f, value =>
            {
                openingCamera.m_Lens.OrthographicSize = value;
            })
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                gameCamera.m_Priority = 100;
                openingCamera.gameObject.SetActive(false);
            });
        }
    }
}