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
        public Queue<EnergyBar> ActiveEnergyBars = new Queue<EnergyBar>(6);
        private Queue<EnergyBar> _inactiveEnergyBars = new Queue<EnergyBar>(6);

        private Coroutine _updateEnergyBars;
        private Tween _bar;
        private EnergyBar _currentEnergyBar;
        private bool _finishedUpdateEnergyBars;
        
        private void Awake()
        {
            InitializeEnergyBars();
        }

        [ContextMenu("Update energy text")]
        public void StartRecharge()
        {
            _updateEnergyBars = StartCoroutine(RechargeEnergyBar());
        }

        public void ResetEnergyBars()
        {
            _bar.Kill();
            StopCoroutine(_updateEnergyBars);
            
            _currentEnergyBar.HideBar();
            
            foreach (EnergyBar energyBar in ActiveEnergyBars)
            {
                energyBar.HideBar();
            }
            
            InitializeEnergyBars();

            StartRecharge();
        }

        public void RemoveCharge(int chargeNumber)
        {
            if (ActiveEnergyBars.Count < chargeNumber) return;

            for (int i = 0; i < chargeNumber; i++)
            {
                EnergyBar x = ActiveEnergyBars.Dequeue();
                _inactiveEnergyBars.Enqueue(x);
                x.HideBar();
            }
            energyText.text = ActiveEnergyBars.Count.ToString();

            if (!_finishedUpdateEnergyBars) return;

            _finishedUpdateEnergyBars = false;
            StartRecharge();
        }
        
        private IEnumerator RechargeEnergyBar()
        {
            while (_inactiveEnergyBars.Count > 0)
            {   
                _currentEnergyBar = _inactiveEnergyBars.Dequeue();
                _currentEnergyBar.transform.SetAsLastSibling();
                _currentEnergyBar.gameObject.SetActive(true);
                _bar = _currentEnergyBar.Recharge();
                yield return _bar.WaitForCompletion();
                ActiveEnergyBars.Enqueue(_currentEnergyBar);
                energyText.text = ActiveEnergyBars.Count.ToString();
            }
            
            _finishedUpdateEnergyBars = true;
        }

        private void InitializeEnergyBars()
        {
            _inactiveEnergyBars.Clear();
            ActiveEnergyBars.Clear();
            energyText.text = "0";
            foreach (EnergyBar energyBar in energyBars)
            {
                _inactiveEnergyBars.Enqueue(energyBar);
            }
        }
    }
}