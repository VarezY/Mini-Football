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
            Color tempColor = _bar.color;
            tempColor.a = .5f;
            _bar.color = tempColor;
            gameObject.SetActive(false);
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
