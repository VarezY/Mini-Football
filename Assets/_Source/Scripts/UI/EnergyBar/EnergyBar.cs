using System;
using DG.Tweening;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFootball.UI
{
    public class EnergyBar : MonoBehaviour
    {
        public float timeToFill;
        private Image _bar;

        private void Awake()
        {
            _bar = GetComponent<Image>();
        }

        private void OnEnable()
        {
            timeToFill = InGameManager.instance.matchManager.timeToFillEnergy;
        }

        public void HideBar()
        {
            _bar.fillAmount = 0;
            _bar.DOFade(0.5f, 0f);
        }

        [ContextMenu("Recharge Energy")]
        public Tween Recharge()
        {
            return _bar.DOFillAmount(1, timeToFill)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
            {
                _bar.DOFade(1, 0f);
            });
        }
    }
}
