using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MiniFootball.UI
{
    public class EnergyBarController : MonoBehaviour
    {
        public EnergyBar[] energyBars;
        public TMP_Text energyText;
        private Queue<EnergyBar> _activeEnergyBars = new Queue<EnergyBar>(6);
        private Queue<EnergyBar> _inactiveEnergyBars = new Queue<EnergyBar>(6);

        private Coroutine _updateEnergyBars;
        private bool _finishedUpdateEnergyBars;
        
        private void Awake()
        {
            _inactiveEnergyBars.Clear();
            _activeEnergyBars.Clear();
            foreach (EnergyBar energyBar in energyBars)
            {
                _inactiveEnergyBars.Enqueue(energyBar);
            }
        }

        [ContextMenu("Update energy text")]
        public void StartRecharge()
        {
            _updateEnergyBars = StartCoroutine(RechageEnergyBar());
        }

        private IEnumerator RechageEnergyBar()
        {
            do
            {
                EnergyBar energyBar = _inactiveEnergyBars.Dequeue();
                energyBar.gameObject.SetActive(true);
                Tween bar = energyBar.Recharge();
                yield return bar.WaitForCompletion();
                _activeEnergyBars.Enqueue(energyBar);
                energyText.text = _activeEnergyBars.Count.ToString();
            } while (_inactiveEnergyBars.Count > 0);

            _finishedUpdateEnergyBars = true;
        }
        
        private void RemoveCharge(int chargeNumber)
        {
            if (_activeEnergyBars.Count < chargeNumber)
                return;

            for (int i = 0; i < chargeNumber; i++)
            {
                EnergyBar x = _activeEnergyBars.Dequeue();
                _inactiveEnergyBars.Enqueue(x);
                x.transform.SetAsLastSibling();
                x.HideBar();
                x.gameObject.SetActive(false);
            }
            energyText.text = _activeEnergyBars.Count.ToString();

            if (!_finishedUpdateEnergyBars) 
                return;

            _finishedUpdateEnergyBars = false;
            StopCoroutine(_updateEnergyBars);
            StartRecharge();
        }

        [ContextMenu("Remove 2 Energy bars")]
        private void RemoveTwoCharge()
        {
            RemoveCharge(2);
        }
    }
}