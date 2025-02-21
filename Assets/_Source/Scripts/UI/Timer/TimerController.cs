using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFootball.UI.Timer
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private int timer;
        [SerializeField] private Image timerOutline;
        [SerializeField] private TMP_Text timerText;

        [ContextMenu("Start")]
        public void StartTimer()
        {
            timerOutline.DOFillAmount(0, timer).SetEase(Ease.Linear);
            DOVirtual.Int(timer, 0, timer, value => timerText.text = $"{value}").SetEase(Ease.Linear);
        }
        
    }
}